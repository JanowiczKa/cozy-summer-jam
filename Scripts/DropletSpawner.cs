using Godot;
using System;

public partial class DropletSpawner : Node2D
{
	public static DropletSpawner Instance { get; private set; }

	[Export] PackedScene dropletScene;
	
	public override void _Ready()
    {
        Instance = this;
    }

	//Still need to add it to the tree and change positions etcs manually, but now we only need one palce to have the packed scene
	public Droplet GetDropletInstance(LiquidData liquid)
	{
		var instance = dropletScene.Instantiate<Droplet>();

		instance.Setup(liquid);

		return instance;
	}
}
