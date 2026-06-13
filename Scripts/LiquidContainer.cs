using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LiquidContainer : RigidBody2D
{
	//1 droplet is 1 part
	[Export] 
	public int containerSize = 10;

	[Export(PropertyHint.File)]
	public Sprite2D liquidSprite;

	private List<LiquidData> liquids = new List<LiquidData>();

	//access to the shader?	
	public override void _Ready()
	{
		liquidSprite.SetInstanceShaderParameter("totalParts", containerSize);
	}
	
	public void AddLiquid(LiquidData newLiquid)
	{
		// return early if already full
		if (liquids.Count() >= containerSize) return;

		liquids.Add(newLiquid);

		UpdateLiquidShader();
	}

	private void UpdateLiquidShader()
	{
		var newVolume = liquids.Count();

        //Shit for performance but small so I don't mind
		var avgRed = liquids.Average(x => x.Color.R);
		var avgGreen = liquids.Average(x => x.Color.G);
		var avgBlue = liquids.Average(x => x.Color.B);
		var avgAlpha = liquids.Average(x => x.Color.A);

		var newColor = new Color(avgRed, avgGreen, avgBlue, avgAlpha);


		liquidSprite.SetInstanceShaderParameter("partsShowing", newVolume);
		liquidSprite.SelfModulate = newColor;

		GD.Print("Updated liquid in glass " + newVolume);
	}
}
