using Godot;

public partial class Garnish : MouseDrag
{
	[Export] public GarnishData garnish;

	public override void _Ready()
	{
        base._Ready();

		BodyEntered += Collided;
	}

	public void Collided(Node body)
	{
		if (body.Name == "DrinkGlass" && body is DrinkContainer)
		{
			Reparent(body);
			Freeze = true;
		}
	}
}
