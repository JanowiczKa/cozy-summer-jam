using Godot;
using System;

public partial class LiquidDispenserButton : Button
{
	[Export] public LiquidData liquid;
	[Export(PropertyHint.FilePath)] public LiquidDispenser parentDispenser;

	public override void _Ready()
	{
		ButtonDown += () => parentDispenser.StartPouringLiquid(liquid);
		ButtonUp += () => parentDispenser.StopPouringLiquid();
	}
}
