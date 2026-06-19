using Godot;
using System;

public partial class CatcherSprite : Sprite2D
{
	[Export] 
	public BottleCatcher parentNode;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Texture = parentNode.tex;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Texture == null)
		{
			Texture = parentNode.tex;
		}
	}
}
