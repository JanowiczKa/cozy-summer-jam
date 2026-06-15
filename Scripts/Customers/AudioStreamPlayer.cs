using Godot;
using System;

public partial class AudioStreamPlayer : Godot.AudioStreamPlayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var textEvent = GetNode<RichTextLabel>("../SpeechBubble/BubbleText");
		textEvent.Connect("VoiceSoundEffect", new Callable(this, MethodName.PlayVoiceSE));
		var spriteEvent = GetNode<CharacterSprite>("../CharacterSprite");
		spriteEvent.Connect("VoiceSoundEffect", new Callable(this, MethodName.PlayVoiceSE));
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
