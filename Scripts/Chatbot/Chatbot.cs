using Godot;
using System;

public partial class Chatbot : Sprite2D
{
	[Export]
	public float velocity;	// 50.0
	private float currentVelocity;
	public bool isAnimated;
	public bool isExtended;
	private ChatBotDialogue dialogueNode;
	private ChatBotRecipe recipeNode;
	private EventController controller;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		isAnimated = false;
		isExtended = false;
		currentVelocity = (float)0.0;
		dialogueNode = GetChild<ChatBotDialogue>(0);
		recipeNode = GetChild<ChatBotRecipe>(1);
		controller = GetNode<EventController>("/root/BarScene/EventController");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isAnimated == true)
		{
			if (isExtended == true)
			{
				AnimateCollapse();
			} else
			{
				AnimateExtension();
			}
		}

	}

	public override void _Input(InputEvent @event)
	{
		// testing purposes only
		if (@event.IsActionPressed("BotExtend"))
		{
			AnimationStart();
		}

		// testing purposes only
		if (@event.IsActionPressed("BotRevert"))
		{
			AnimationStart();
		}
	}

	public void AnimationStart()
	{
		if (controller.gmstate == EventController.GameState.Gameplay)
		{
			isAnimated = true;
			currentVelocity = velocity;
		}
	}

	private void TerminateAnimation()
	{
		isAnimated = false;
		currentVelocity = (float)0.0;
		if (isExtended == false)
		{
			isExtended = true;
		} else
		{
			isExtended = false;
		}

	}

	private void AnimateExtension()
	{
		Position = new Vector2(Position.X, Position.Y + currentVelocity);
		currentVelocity = currentVelocity /2;
		float difference = currentVelocity / velocity;

		dialogueNode.ChangeOpacity(dialogueNode.Modulate.A - difference,
		dialogueNode.Modulate.B - difference,
		dialogueNode.Modulate.G - difference,
		dialogueNode.Modulate.R - difference);

		recipeNode.ChangeOpacity(recipeNode.Modulate.A + difference,
		recipeNode.Modulate.B + difference,
		recipeNode.Modulate.G + difference,
		recipeNode.Modulate.R + difference);

		if (currentVelocity <= 1)
		{
			TerminateAnimation();
		}
		
	}

	private void AnimateCollapse()
	{
		Position = new Vector2(Position.X, Position.Y - currentVelocity);
		currentVelocity = currentVelocity /2;
		float difference = currentVelocity / velocity;

		dialogueNode.ChangeOpacity(dialogueNode.Modulate.A + difference,
		dialogueNode.Modulate.B + difference,
		dialogueNode.Modulate.G + difference,
		dialogueNode.Modulate.R + difference);

		recipeNode.ChangeOpacity(recipeNode.Modulate.A - difference,
		recipeNode.Modulate.B - difference,
		recipeNode.Modulate.G - difference,
		recipeNode.Modulate.R - difference);

		if (currentVelocity <= 1)
		{
			TerminateAnimation();
		}
		
	}
}
