using Godot;

[GlobalClass]
public partial class LiquidData : Resource
{
	[Export]
	public string LiquidName;

	[Export]
	public Color Color;

	[Export]
	public bool IsFizzy;

	public LiquidData() : this("", new Color(1,1,1,1), false) {}

	public LiquidData(string liquidName, Color color, bool isFizzy)
	{
		LiquidName = liquidName;
		Color = color;
		IsFizzy = isFizzy;
	}
}
