using Godot;
using System;
using System.Linq;

public partial class BottleCatcher : Area2D
{
	public MouseDrag bottle;
	public Texture2D tex;
	public VisibleOnScreenNotifier2D notif;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bottle = GetParent<MouseDrag>();
		notif = bottle.GetChild<VisibleOnScreenNotifier2D>(3);
		LiquidData liquid = bottle.GetMeta("Liquid").As<LiquidData>();
		tex = liquid.bottle;
		//GD.Print("Texture: " + tex);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (OverlapsBody(bottle) == true && bottle.isHeld == false)
		{
			bottle.Rotation = (float)0.0;
			bottle.Freeze = true;
			bottle.GlobalPosition = new Vector2(this.GlobalPosition.X, this.GlobalPosition.Y);
		}

		if (notif.IsOnScreen() == false && bottle.isHeld == false)
		{
			bottle.Rotation = (float)0.0;
			bottle.Freeze = true;
			bottle.GlobalPosition = new Vector2(this.GlobalPosition.X, this.GlobalPosition.Y);
		}
		
	}

}
