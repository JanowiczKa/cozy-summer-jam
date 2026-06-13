using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class CharacterSprite : Sprite2D
{
	// Some of these will have to be removed, redundancy prevention lol
	private bool _isAnimated;
	private double total_time_elapsed;
	private int animation_sequence;
	private double pos_acceleration, neg_acceleration;
	private double velocity;
	private double max_velocity, min_velocity;
	private Color rgb_modulation;
	private enum AnimationType
	{
		FadeIn,
		FadeOut,
		Bounce,
		Static
	};
	private AnimationType animtp;

	[Signal]
	private delegate void FadeInFinishedEventHandler();
	[Signal]
	private delegate void FadeOutFinishedEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Scale = new Vector2((float)0.2, (float)0.2);
		_isAnimated = false;
		total_time_elapsed = 0.0;
		animation_sequence = 0;
		pos_acceleration = 0.0;
		neg_acceleration = 0.0;
		velocity = 0.0;
		max_velocity = 0.0;
		min_velocity = 0.0;
		rgb_modulation = new Color(0, 0, 0, 0);
		Modulate = rgb_modulation;
		animtp = AnimationType.Static;

		var parentNode = GetParent<Node2D>();
		parentNode.Connect("DialogAction", new Callable(this, MethodName.InitBounceAnimation));
		parentNode.Connect("FadeInAction", new Callable(this, MethodName.InitFadeInAnimation));
		parentNode.Connect("FadeOutAction", new Callable(this, MethodName.InitFadeOutAnimation));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		total_time_elapsed += delta;
		
		// Animation loop (sort of)
		if (_isAnimated)
		{
			switch(animtp)
			{
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
		_isAnimated = true;
		animtp = AnimationType.FadeIn;
	}

	private void InitFadeOutAnimation()
	{
		_isAnimated = true;
		animtp = AnimationType.FadeOut;
	}
	
	// Initiates the sentence animation and sets its values
	private void InitBounceAnimation(string expression, string txt)
	{
		if (expression != "")
		{
			_isAnimated = true;
			animtp = AnimationType.Bounce;
			velocity 	 = 0.001;
			neg_acceleration = 0.00001;
			pos_acceleration = 0.00001;
			max_velocity = 0.01;
			min_velocity = 0.0001;
		}
	}

	private void TerminateBounceAnimation()
	{
		animation_sequence = 0;
		animtp = AnimationType.Static;
		_isAnimated = false;
		velocity 	 = 0.001;
		neg_acceleration = 0.0;
		pos_acceleration = 0.0;
		max_velocity = 0.0;
		min_velocity = 0.0;
	}
	
	// Manipulates the sprite's Scale values to stretch the image
	// every game loop iteration
	private void AnimateBounce()
	{
		// List of a maximum and minimum distance to stretch the sprite
		List<double> sentence_start_anim = [0.21, 0.2];
		
		// Sprite manipulation towards the maximum stretch distance
		if (animation_sequence == 0)
		{
			if ((double)Scale.Y < sentence_start_anim[animation_sequence])
			{
				// Add the velocity to the Y value of Scale
				Scale = new Vector2(Scale.X, Scale.Y + (float)velocity);
				
				// Recalculate the velocity to decrease speed at which
				// the sprite is stretched
				velocity -= neg_acceleration;
				if (velocity < min_velocity)
				{
					velocity = min_velocity;
				}
			}
			else
			{
				// Move on to the next sequence of the animation
				animation_sequence += 1;
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
				velocity += pos_acceleration;
				if (velocity > max_velocity)
				{
					velocity = max_velocity;
				}
			}
			else
			{
				TerminateBounceAnimation();
			}
		}
	}

	private void AnimateFadeIn()
	{
		if (total_time_elapsed >= 1.0)
		{
			if (rgb_modulation.A < 1.0)
				{
				ChangeRGBValues(rgb_modulation.A+(float)0.25, rgb_modulation.B+(float)0.25, rgb_modulation.G+(float)0.25, rgb_modulation.R+(float)0.25);
				total_time_elapsed = 0.0;
				}
			else
				{
				_isAnimated = false;
				animtp = AnimationType.Static;
				EmitSignal(SignalName.FadeInFinished);
				}
		}
	}

	private void AnimateFadeOut()
	{
		GD.Print("Fade out");
		if (total_time_elapsed >= 1.0)
		{
			if (rgb_modulation.A <= 1.0 && rgb_modulation.A != 0.0)
				{
				ChangeRGBValues(rgb_modulation.A-(float)0.25, rgb_modulation.B-(float)0.25, rgb_modulation.G-(float)0.25, rgb_modulation.R-(float)0.25);
				total_time_elapsed = 0.0;
				}
			else
				{
				_isAnimated = false;
				animtp = AnimationType.Static;
				EmitSignal(SignalName.FadeOutFinished);
				}
		}
	}
}
