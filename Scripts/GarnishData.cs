using Godot;

[GlobalClass]
public partial class GarnishData : Resource
{
	[Export]
	public string GarnishName;
	[Export]
	public LiquidData GarnishToLiquid;

	public GarnishData() : this("", null) {}

	public GarnishData(string garnishName, LiquidData liquidData)
	{
		GarnishName = garnishName;
		GarnishToLiquid = liquidData;
	}
}
