using Godot;

public partial class DrinkContainer : MouseDrag
{
	[Export(PropertyHint.File)]
	public LiquidContainer liquidContainer;

	[Export(PropertyHint.File)]
	public GarnishObserver garnishObserver;
}
