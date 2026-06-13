using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Node2D
{
	[Signal]
	private delegate void DialogActionEventHandler(string expression, string speech);
	[Signal]
	private delegate void FadeInActionEventHandler();
	[Signal]
	private delegate void FadeOutActionEventHandler();
	[Signal]
	private delegate void ControllerSequenceEndEventHandler(string sequence);
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var spriteNode = GetNode<Sprite2D>("./CharacterSprite");
		spriteNode.Connect("FadeInFinished", new Callable(this, MethodName.StartDialogWhenFinishedFadingIn));
		spriteNode.Connect("FadeOutFinished", new Callable(this, MethodName.StartDialogWhenFinishedFadingOut));
		var controllerNode = GetNode<Node>("../EventController");
		controllerNode.Connect("SkipDialogLine", new Callable(this, MethodName.PlayDialog));
		controllerNode.Connect("StartFadeIn", new Callable(this, MethodName.StartFadeInSequence));
		controllerNode.Connect("StartFadeOut", new Callable(this, MethodName.StartFadeOutSequence));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PlayDialog(string expression, string speech)
	{
		EmitSignal(SignalName.DialogAction, expression, speech);
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
