using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterSprite : Sprite2D
{
	// Some of these will have to be removed, redundancy prevention lol
	private bool _isAnimated;
	private double _animationTime;
	private double total_time_elapsed;
	private List<double> sentence_start_anim;
	private int animation_sequence;
	private double pos_acceleration, neg_acceleration;
	private double velocity;
	private double max_velocity, min_velocity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Scale = new Vector2((float)0.2, (float)0.2);
		_isAnimated = false;
		_animationTime = 2.0;
		total_time_elapsed = 0.0;
		animation_sequence = 0;
		pos_acceleration = 0.0;
		neg_acceleration = 0.0;
		velocity = 0.0;
		max_velocity = 0.0;
		min_velocity = 0.0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		total_time_elapsed += delta;
		// Input to test the animation
		if (Input.IsActionJustPressed("ui_select") && _isAnimated == false)
		{
			_StartSentenceAnimation();
		}
		
		// Animation loop (sort of)
		if (_isAnimated)
		{
			_AnimateSentenceStart();
		}
	}
	
	// Initiates the sentence animation and sets its values
	public void _StartSentenceAnimation()
	{
		_isAnimated = true;
		velocity 	 = 0.001;
		neg_acceleration = 0.00001;
		pos_acceleration = 0.00001;
		max_velocity = 0.01;
		min_velocity = 0.0001;
	}
	
	// Manipulates the sprite's Scale values to stretch the image
	// every game loop iteration
	private void _AnimateSentenceStart()
	{
		// List of a maximum and minimum distance to stretch the sprite
		List<double> sentence_start_anim = [0.22, 0.2];
		
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
				// Reset animation values when the animation is finished
				animation_sequence = 0;
				_isAnimated = false;
				velocity 	 = 0.001;
				neg_acceleration = 0.0;
				pos_acceleration = 0.0;
				max_velocity = 0.0;
				min_velocity = 0.0;
			}
		}
	}
}
