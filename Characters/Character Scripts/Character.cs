using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Node2D
{
	private List<string> expression_sequence;
	private List<string> speech_sequence;
	private int sequence_index;
	[Signal]
	private delegate void DialogActionEventHandler(string expression, string speech);
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		expression_sequence = ["DefaultExp", "DefaultExp", "", "Mog"];
		speech_sequence = ["Ay it's me, Sans Undertale over here", "From the award winning game Undertale", "...", "You doing anything tonight?"];
		sequence_index = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Space"))
		{
			EmitSignal(SignalName.DialogAction, expression_sequence[sequence_index], speech_sequence[sequence_index]);
			sequence_index += 1;
			if (sequence_index >= expression_sequence.Count || sequence_index >= speech_sequence.Count)
			{
				sequence_index = 0;
			}
		}
	}
}
