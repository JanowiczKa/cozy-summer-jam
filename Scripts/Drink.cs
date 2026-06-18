using Godot;

[GlobalClass]
public partial class Drink : Resource
{
	[Export]
	public string DrinkName {get; set;}
	[Export]
	public LiquidData[] DrinkList {get; set;}

	public Drink() : this("", null) {}

	public Drink(string name, LiquidData[] drink)
	{
		DrinkName = name;
		DrinkList = drink;
	}
}
