using Godot;
using System;

public partial class AudioStreamPlayer : Godot.AudioStreamPlayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var audioStream = GetNode<RichTextLabel>("../SpeechBubble/BubbleText");
		audioStream.Connect("VoiceSoundEffect", new Callable(this, MethodName.PlayVoiceSE));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void PlayVoiceSE()
	{
		Play((float)0.0);
	}
	
	
}
