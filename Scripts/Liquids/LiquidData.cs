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

	[Export]
	public Texture2D bottle;

	public LiquidData() : this("", new Color(1,1,1,1), false, null) {}

	public LiquidData(string liquidName, Color color, bool isFizzy, Texture2D bottleSprite)
	{
		LiquidName = liquidName;
		Color = color;
		IsFizzy = isFizzy;
		bottle = bottleSprite;
	}
}
