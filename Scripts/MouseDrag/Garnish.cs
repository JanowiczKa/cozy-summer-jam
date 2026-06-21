using System.Linq;
using Godot;

public partial class Garnish : MouseDrag
{
	[Export] public GarnishData garnishData;

    public bool isInGlass;
    public int xOffset;

    public override void _Process(double delta)
    {
        if (isInGlass)
        {
            LiquidContainer parent = GetParent<LiquidContainer>();
            int offset = parent.currentVolume * 4;
            GlobalPosition = new Vector2(parent.GlobalPosition.X+xOffset, parent.GlobalPosition.Y+30-offset);
            GlobalRotationDegrees = parent.GlobalRotationDegrees;
        }
    }

}
