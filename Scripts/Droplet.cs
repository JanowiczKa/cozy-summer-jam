using Godot;

public partial class Droplet : RigidBody2D
{
	[Export] public LiquidData liquid;
	
	//hue variance

	[Export] public float timeToLive = 5f;

	private double timer;

	public void Setup(LiquidData liquidData)
	{
		liquid = liquidData;
	}

	public override void _Ready()
	{
		if (liquid == null)
		{
			GD.PrintErr("Droplet doesn't have liquid assigned!");
			QueueFree();
		}

		var childSprite = GetNode<Sprite2D>("Sprite2D");
		if (childSprite != null) childSprite.Modulate = liquid.Color;

		BodyEntered += Collided;
	}

	public void Collided(Node body)
	{
		if (body.Name == "DrinkGlass" && body is DrinkContainer)
		{
			//GD.Print("liquids +1 " + liquid.LiquidName);

			var liquidContainer = body as DrinkContainer;

			liquidContainer.AddLiquid(liquid);

			QueueFree();
		}

		if (body.Name == "Table")
		{
			//GD.Print("Hit the table :(");
			QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer += delta;

		if (timer > timeToLive)
		{
			QueueFree();
		}
	}
}
