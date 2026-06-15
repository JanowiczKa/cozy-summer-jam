using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Node2D
{
	[Signal]
	private delegate void PlayDrinkingAnimationEventHandler();
	[Signal]
	private delegate void SetDialogEventHandler(string speech);
	[Signal]
	private delegate void SetExpressionEventHandler(string expression);
	[Signal]
	private delegate void PlayBounceAnimationEventHandler();
	[Signal]
	private delegate void FadeInActionEventHandler();
	[Signal]
	private delegate void FadeOutActionEventHandler();
	[Signal]
	private delegate void TextClearEventHandler();
	[Signal]
	private delegate void ExpressionClearEventHandler();
	[Signal]
	private delegate void ControllerSequenceEndEventHandler(string sequence);
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var spriteNode = GetNode<Sprite2D>("./CharacterSprite");
		spriteNode.Connect("FadeInFinished", new Callable(this, MethodName.StartDialogWhenFinishedFadingIn));
		spriteNode.Connect("FadeOutFinished", new Callable(this, MethodName.StartDialogWhenFinishedFadingOut));
		var controllerNode = GetNode<Node>("../EventController");
		controllerNode.Connect("StartNextDialog", new Callable(this, MethodName.PlayDialog));
		controllerNode.Connect("StartBounceAnimation", new Callable(this, MethodName.AnimateBouncing));
		controllerNode.Connect("ChangeExpression", new Callable(this, MethodName.PlayExpression));
		controllerNode.Connect("ClearDialogAndExpression", new Callable(this, MethodName.SetDialogAndExpressionToDefault));
		controllerNode.Connect("StartDrinkingAnimation", new Callable(this, MethodName.AnimateDrinking));
		controllerNode.Connect("StartFadeIn", new Callable(this, MethodName.StartFadeInSequence));
		controllerNode.Connect("StartFadeOut", new Callable(this, MethodName.StartFadeOutSequence));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PlayExpression(string expression)
	{
		EmitSignal(SignalName.SetExpression, expression);
	}

	private void AnimateBouncing()
	{
		EmitSignal(SignalName.PlayBounceAnimation);
	}

	private void PlayDialog(string speech)
	{
		EmitSignal(SignalName.SetDialog, speech);
	}

	private void AnimateDrinking()
	{
		EmitSignal(SignalName.PlayDrinkingAnimation);
	}

	private void SetDialogAndExpressionToDefault()
	{
		EmitSignal(SignalName.TextClear);
		EmitSignal(SignalName.ExpressionClear);
	}

	private void StartDialogWhenFinishedFadingIn()
	{
		EmitSignal(SignalName.ControllerSequenceEnd, "Introduction");
	}

	private void StartDialogWhenFinishedFadingOut()
	{
		EmitSignal(SignalName.ControllerSequenceEnd, "End");
	}


	private void StartFadeInSequence()
	{
		EmitSignal(SignalName.FadeInAction);
	}

	private void StartFadeOutSequence()
	{
		EmitSignal(SignalName.FadeOutAction);
	}
}
