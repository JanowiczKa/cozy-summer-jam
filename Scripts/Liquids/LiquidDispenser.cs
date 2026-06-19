using Godot;

public partial class LiquidDispenser : Node2D
{
	[Export] public float dropsPerSecondRate = 30;
	[Export] public float spillRadius = 5f;
	[Export] public Node2D spillPosition;

	private LiquidData currentLiquid;

	private RandomNumberGenerator rand;

	private double timer;

	public override void _Ready()
	{
		rand = new RandomNumberGenerator();
	}

	public void StartPouringLiquid(LiquidData liquid)
	{
		currentLiquid = liquid;
		timer = 0;
	}

	public void StopPouringLiquid()
	{
		currentLiquid = null;
	}

	public override void _Process(double delta)
	{
		if (currentLiquid == null) return;

		timer += delta;

		var timeBetweenDrops = 1 / dropsPerSecondRate;

		if (timer >= timeBetweenDrops)
		{
			timer = 0;

			var dropletInstance = DropletSpawner.Instance.GetDropletInstance(currentLiquid);

			var randomOffset = new Vector2(rand.RandfRange(-1, 1), rand.RandfRange(-1, 1)).Normalized() * spillRadius;

			var spawnPosition = spillPosition.GlobalPosition + randomOffset;

			dropletInstance.GlobalPosition = spawnPosition;

			GetTree().Root.AddChild(dropletInstance);
		}
	}

}
