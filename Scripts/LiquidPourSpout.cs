using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class LiquidPourSpout : Node2D
{
	[Export] public Node2D parentNode;
	[Export] public LiquidData liquid;
	[Export] PackedScene dropletScene;

	[Export] public float directionalForceWhenInstanciated = 130f;
	[Export] public float spillRadius = 5f;
	[Export] [Range(0, 180)] public float spillAngle = 75f;
	private float spillAngleCos;

	[Export] public float spillRatePerSecond = 10f;
	// [Export] public float spillRatePerSecondMin = 5f;
	// [Export] public float spillRatePerSecondMax = 10f;

	private double timer;

	private Droplet dropletInsance;

	private RandomNumberGenerator rand;

    public override void _Ready()
    {
		liquid = parentNode.GetMeta("Liquid").As<LiquidData>();

		spillAngleCos = MathF.Cos(spillAngle * (MathF.PI / 180f));

		rand = new RandomNumberGenerator();
    }

	//based on nozzle node position from centre of it's scene 
	private Vector2 CurrentDirection() => (GlobalPosition - parentNode.GlobalPosition).Normalized();

	private bool IsWithinPourAngle()
	{
		var dotProduct = Vector2.Down.Dot(CurrentDirection());

		return dotProduct > spillAngleCos; 
	}

	private bool EnoughTimeElapsedToSpawnNextDroplet()
	{
		var currentDropletSpawnTimer = 1 / spillRatePerSecond;

        //if pointing closer to directly down the spawn rate should increase maybe?
		if (timer < currentDropletSpawnTimer) return false;

		return true;
	}

	private void SpawnDroplet()
	{
		timer = 0;

		var instance = dropletScene.Instantiate<Droplet>();

		instance.Setup(liquid);

		var randomOffset = new Vector2(rand.RandfRange(-1, 1), rand.RandfRange(-1, 1)).Normalized() * spillRadius;

		//GD.Print(randomOffset);

		var spawnPosition = GlobalPosition + randomOffset;

		instance.GlobalPosition = spawnPosition;
		instance.ApplyImpulse(CurrentDirection() * directionalForceWhenInstanciated);

		GetTree().Root.AddChild(instance);

		//insanciate droplet scene at spout position
		//add velocity maybe?
	}

	public override void _Process(double delta)
	{
		timer += delta;

		if (!IsWithinPourAngle()) return;

		if (!EnoughTimeElapsedToSpawnNextDroplet()) return;

		SpawnDroplet();
	}
}
