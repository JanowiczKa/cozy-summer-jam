using Godot;
using System;
using System.Collections.Generic;

public partial class EventController : Node
{
	private enum GameState
	{
		Idle,
		Introduction,
		Gameplay,
		Result
	}
	private List<string> expression_sequence;
	private List<string> speech_sequence;
	private int sequence_index;
	private double total_time_elapsed;
	private GameState gmstate { get; set; }
	private Random rand;

	[Signal]
	private delegate void SkipDialogLineEventHandler(string expression, string speech);
	[Signal]
	private delegate void StartFadeInEventHandler();
	[Signal]
	private delegate void StartFadeOutEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		expression_sequence = [];
		speech_sequence = [];
		sequence_index = 0;
		total_time_elapsed = 0.0;
		rand = new Random();

		gmstate = GameState.Idle;
		var charactersNode = GetNode<Node2D>("../Characters");
		charactersNode.Connect("ControllerSequenceEnd", new Callable(this, MethodName.ChangeGameState));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		total_time_elapsed += delta;

		if (gmstate == GameState.Gameplay && total_time_elapsed >= 5.0)
		{
			sequence_index = rand.Next(expression_sequence.Count);
			EmitSignal(SignalName.SkipDialogLine, expression_sequence[sequence_index], speech_sequence[sequence_index]);
			total_time_elapsed = 0.0;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Space") && (gmstate == GameState.Introduction || gmstate == GameState.Result))
		{
			if (sequence_index >= expression_sequence.Count || sequence_index >= speech_sequence.Count)
			{
				sequence_index = 0;
				if (gmstate == GameState.Introduction)
				{
					ChangeGameState("Gameplay");
					EmitSignal(SignalName.SkipDialogLine, "DefaultExp", "");
				}
				else
				{
					EmitSignal(SignalName.StartFadeOut);
					EmitSignal(SignalName.SkipDialogLine, "", "");
				}
				return;
			}
			EmitSignal(SignalName.SkipDialogLine, expression_sequence[sequence_index], speech_sequence[sequence_index]);
			sequence_index += 1;
		}

		if (@event.IsActionPressed("FadeOut"))
		{
			ChangeGameState("Result");
			EmitSignal(SignalName.SkipDialogLine, expression_sequence[sequence_index], speech_sequence[sequence_index]);
			sequence_index += 1;
		}

		if (@event.IsActionPressed("FadeIn"))
		{
			EmitSignal(SignalName.StartFadeIn);
		}
	}
	
	private void ChangeGameState(string sequence)
	{
		switch (sequence)
		{
			case "Introduction":
				gmstate = GameState.Introduction;
				expression_sequence = ["DefaultExp", "DefaultExp", "", "Mog"];
				speech_sequence = ["Ay it's me, Sans Undertale over here", "Can I get uhh...",
		 		"...", "Seven million shots of Absinthe Father?"];
				EmitSignal(SignalName.SkipDialogLine, expression_sequence[sequence_index], speech_sequence[sequence_index]);
				sequence_index += 1;
				break;
			case "Gameplay":
				gmstate = GameState.Gameplay;
				speech_sequence = ["Did I mention I'm from the award winning game Undertale?", "Yeah mix it good baby", "I'm Sans Undertale"
				, "Tell Shaun I said hi", "I wonder what else those hands can do"];
				expression_sequence = ["DefaultExp", "Mog", "DefaultExp", "DefaultExp", "Mog"];
				total_time_elapsed = 0.0;
				break;
			case "Result":
				gmstate = GameState.Result;
				speech_sequence = ["Cheers boss, lemme have a sip", "Oh yeah", "This shit sucks", "Smell ya later, sexy"];
				expression_sequence = ["DefaultExp", "DefaultExp", "DefaultExp", "Mog"];
				sequence_index = 0;
				break;
			case "End":
				gmstate = GameState.Idle;
				sequence_index = 0;
				break;
		}
	}

}
