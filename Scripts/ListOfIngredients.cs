using Godot;

[GlobalClass]
public partial class ListOfIngredients : Resource
{
	[Export]
	public LiquidData[] liquids;

	public ListOfIngredients() : this(null) {}

	public ListOfIngredients(LiquidData[] liquidList)
	{
		liquids = liquidList;
	}
}
