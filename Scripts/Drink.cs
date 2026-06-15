using Godot;

[GlobalClass]
public partial class Drink : Resource
{
    [Export]
    public LiquidData[] DrinkList {get; set;}

    public Drink() : this(null) {}

    public Drink(LiquidData[] drink)
    {
        DrinkList = drink;
    }
}