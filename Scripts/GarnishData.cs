using Godot;

[GlobalClass]
public partial class GarnishData : Resource
{
	[Export]
	public string GarnishName;

	public GarnishData() : this("") {}

	public GarnishData(string garnishName)
	{
		GarnishName = garnishName;
	}
}
