using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class CharacterSprite : Sprite2D
{
	// Some of these will have to be removed, redundancy prevention lol
	public bool is_animated;
	private double total_time_elapsed;
	private int animation_sequence;
	private double acceleration;
	private double velocity;
	private double max_velocity, min_velocity;
	private Color rgb_modulation;
	private int iterations;
	private List<double> anim_bounds;
	private enum AnimationType
	{
		FadeIn,
		FadeOut,
		Bounce,
		Drink,
		Static
	};
	private AnimationType animtp;
	private Vector2 baseSize;

	[Signal]
	private delegate void FadeInFinishedEventHandler();
	//[Signal]
	//private delegate void FadeOutFinishedEventHandler();
	[Signal]
	private delegate void VoiceSoundEffectEventHandler();
	[Signal]
	private delegate void DrinkingFinishedEventHandler(string sequence);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Scale = new Vector2((float)0.36, (float)0.36);
		baseSize = new Vector2(Scale.X, Scale.Y);
		is_animated = false;
		total_time_elapsed = 0.0;
		animation_sequence = 0;
		acceleration = 0.0;
		velocity = 0.0;
		max_velocity = 0.0;
		min_velocity = 0.0;
		iterations = 0;
		rgb_modulation = new Color(0, 0, 0, 0);
		Modulate = rgb_modulation;
		animtp = AnimationType.Static;
		anim_bounds = [Scale.X + 0.02, Scale.Y];

		var parentNode = GetParent<Node2D>();
		parentNode.Connect("PlayBounceAnimation", new Callable(this, MethodName.InitBounceAnimation));
		parentNode.Connect("FadeInAction", new Callable(this, MethodName.InitFadeInAnimation));
		parentNode.Connect("FadeOutAction", new Callable(this, MethodName.InitFadeOutAnimation));
		parentNode.Connect("PlayDrinkingAnimation", new Callable(this, MethodName.InitDrinkAnimation));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		total_time_elapsed += delta;
		
		// Animation loop (sort of)
		if (is_animated)
		{
			switch(animtp)
			{
				case AnimationType.Drink:
				case AnimationType.Bounce:
					AnimateBounce();
					break;
				case AnimationType.FadeIn:
					AnimateFadeIn();
					break;
				case AnimationType.FadeOut:
					AnimateFadeOut();
					break;
				case AnimationType.Static:
					break;
			}
		}
	}

	private void ChangeRGBValues(float a, float b, float g, float r)
	{
		rgb_modulation.A = a;
		rgb_modulation.B = b;
		rgb_modulation.G = g;
		rgb_modulation.R = r;
		Modulate = rgb_modulation;
	}

	private void InitFadeInAnimation()
	{
		is_animated = true;
		animtp = AnimationType.FadeIn;
	}

	private void InitFadeOutAnimation()
	{
		is_animated = true;
		animtp = AnimationType.FadeOut;
	}
	
	// Initiates the sentence animation and sets its values
	private void InitBounceAnimation()
	{
		is_animated = true;
		animtp = AnimationType.Bounce;
		velocity 	 = 0.001;
		acceleration = 0.00001;
		max_velocity = 0.01;
		min_velocity = 0.0001;
		anim_bounds = [baseSize.X + 0.02, baseSize.Y];
	}

	private void InitDrinkAnimation()
	{
		is_animated = true;
		animtp = AnimationType.Drink;
		velocity 	 = 0.001;
		acceleration = 0.00001;
		max_velocity = 0.01;
		min_velocity = 0.0001;
		anim_bounds = [baseSize.X + 0.04, baseSize.Y];
	}

	private void TerminateBounceAnimation()
	{
		animation_sequence = 0;
		animtp = AnimationType.Static;
		is_animated = false;
		velocity 	 = 0.001;
		acceleration = 0.0;
		max_velocity = 0.0;
		min_velocity = 0.0;
		anim_bounds = [baseSize.X, baseSize.Y];
	}
	
	// Manipulates the sprite's Scale values to stretch the image
	// every game loop iteration
	private void AnimateBounce()
	{
		// List of a maximum and minimum distance to stretch the sprite
		List<double> sentence_start_anim = anim_bounds;
		
		// Sprite manipulation towards the maximum stretch distance
		if (animation_sequence == 0)
		{
			if ((double)Scale.Y < sentence_start_anim[animation_sequence])
			{
				// Add the velocity to the Y value of Scale
				Scale = new Vector2(Scale.X, Scale.Y + (float)velocity);
				
				// Recalculate the velocity to decrease speed at which
				// the sprite is stretched
				velocity -= acceleration;
				if (velocity < min_velocity)
				{
					velocity = min_velocity;
				}
			}
			else
			{
				// Move on to the next sequence of the animation
				animation_sequence += 1;

				// If the animation is set to Drinking, play the speech sound effect at the apex of the animation
				if (animtp == AnimationType.Drink)
					EmitSignal(SignalName.VoiceSoundEffect);
			}
		}
		// Sprite manipulation towards the starting dimensions
		else if (animation_sequence == 1)
		{
			if ((double)Scale.Y > sentence_start_anim[animation_sequence])
			{
				// Add the velocity to the Y value of Scale
				Scale = new Vector2(Scale.X, Scale.Y - (float)velocity);
				
				// Recalculate the velocity to increase speed at which
				// the sprite is stretched
				velocity += acceleration;
				if (velocity > max_velocity)
				{
					velocity = max_velocity;
				}
			}
			else
			{
				if (animtp == AnimationType.Drink && iterations < 2)
				{
					animation_sequence = 0;
					iterations += 1;
					velocity = 0.001;
				}
				else
				{
					// If the animation is set to Drinking, send signal to controller to trigger the next game state
					if (animtp == AnimationType.Drink)
						EmitSignal(SignalName.DrinkingFinished, "Result");
					iterations = 0;
					TerminateBounceAnimation();
				}
			}
		}
	}

	private void AnimateFadeIn()
	{
		if (total_time_elapsed >= 0.7)
		{
			if (rgb_modulation.A < 1.0)
				{
				ChangeRGBValues(rgb_modulation.A+(float)0.25, rgb_modulation.B+(float)0.25, rgb_modulation.G+(float)0.25, rgb_modulation.R+(float)0.25);
				total_time_elapsed = 0.0;
				}
			else
				{
				is_animated = false;
				animtp = AnimationType.Static;
				EmitSignal(SignalName.FadeInFinished);
				}
		}
	}

	private void AnimateFadeOut()
	{
		GD.Print("Fade out");
		if (total_time_elapsed >= 0.7)
		{
			if (rgb_modulation.A <= 1.0 && rgb_modulation.A != 0.0)
				{
				ChangeRGBValues(rgb_modulation.A-(float)0.25, rgb_modulation.B-(float)0.25, rgb_modulation.G-(float)0.25, rgb_modulation.R-(float)0.25);
				total_time_elapsed = 0.0;
				}
			else
				{
				is_animated = false;
				animtp = AnimationType.Static;
				//EmitSignal(SignalName.FadeOutFinished);
				}
		}
	}
}
