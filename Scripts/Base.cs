using Godot;
using System;

public partial class Base : Sprite2D
{
	[Export] 
	public Node2D parentNode;
	private LiquidData liquid;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		liquid = parentNode.GetMeta("Liquid").As<LiquidData>();
		Texture = liquid.bottle;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
