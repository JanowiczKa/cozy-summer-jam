using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LiquidContainer : Area2D //MouseDrag
{
	//1 droplet is 1 part
	[Export] 
	public int containerSize = 60;
	public int currentVolume;
	public Random rand;

	[Export(PropertyHint.File)]
	public Sprite2D liquidSprite;

	public List<LiquidData> liquids = new List<LiquidData>();

	//access to the shader?	
	public override void _Ready()
	{
		liquidSprite.SetInstanceShaderParameter("totalParts", containerSize);

		BodyEntered += Collision;
		rand = new Random();
		currentVolume = 0;
	}

	public void Collision(Node body)
	{
		//GD.Print("something entered the glass! " + body.Name);

		if (body is Droplet)
		{
			var droplet = body as Droplet;

			AddLiquid(droplet.liquid);

			body.QueueFree();
		}

		if (body is Garnish)
		{
			// return early if already full
			if (liquids.Count() >= containerSize) return;
			
			Garnish garnish = body as Garnish;
			
			if (garnish.isHeld != true)
			{
				AddGarnish(garnish.garnishData.GarnishToLiquid);

				var garnishParent = garnish.GetParent();
				garnishParent.RemoveChild(garnish);
				CallDeferred("add_child", garnish);
				garnish.SetDeferred("freeze", true);
				int offset = rand.Next(-15,15);
				garnish.SetDeferred("global_position", new Vector2(GlobalPosition.X+offset, GlobalPosition.Y));
				garnish.CallDeferred("remove_child", garnish.GetChild(1));
				garnish.SetDeferred("isInGlass", true);
				garnish.SetDeferred("xOffset", offset);
			}
		}
	}
	
	public void AddLiquid(LiquidData newLiquid)
	{
		// return early if already full
		if (liquids.Count() >= containerSize) return;

		liquids.Add(newLiquid);
		currentVolume++;

		UpdateLiquidShader();
	}

	public void AddGarnish(LiquidData newLiquid)
	{
		liquids.Add(newLiquid);
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
