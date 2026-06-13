using Godot;
using System;
using System.Collections.Generic;

public partial class RichTextLabel : Godot.RichTextLabel
{
	private double total_time_elapsed;
	private int text_index;
	private int sequence_index;
	[Signal]
	private delegate void VoiceSoundEffectEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		total_time_elapsed = 0.0;
		VisibleCharacters = 0;
		text_index = 0;

		var parentNode = GetNode<Node2D>("../..");
		parentNode.Connect("DialogAction", new Callable(this, MethodName.LoadNewLine));
		parentNode.Connect("FadeOutAction", new Callable(this, MethodName.ClearText));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		total_time_elapsed += delta;
		
		if (total_time_elapsed >= 0.07 && VisibleCharacters < Text.Length)
		{
			string blank_space_check = Text;
			if (blank_space_check[VisibleCharacters] != ' ' && blank_space_check[VisibleCharacters] != '.')
				EmitSignal(SignalName.VoiceSoundEffect);
			VisibleCharacters += 1;
			total_time_elapsed = 0.0;
		}
	}

	private void LoadNewLine(string expression, string txt)
	{
		VisibleCharacters = 0;
		Text = txt;	
	}

	private void ClearText()
	{
		VisibleCharacters = 0;
		Text = "";
	}
}
