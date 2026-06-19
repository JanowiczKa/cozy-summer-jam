using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ChatBotRecipe : RichTextLabel
{
	[Export]
	public ListOfIngredients ingredientList;
	private EventController controller;
	private Random rand;
	private Color rgb_modulation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		controller = GetNode<EventController>("/root/BarScene/EventController");
		Text = "";
		rand = new Random();
		rgb_modulation = new Color((float)0.0, (float)0.0, (float)0.0, (float)0.0);
		Modulate = rgb_modulation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ChangeOpacity(float a, float b, float g, float r)
	{
		rgb_modulation.A = a;
		rgb_modulation.B = b;
		rgb_modulation.G = g;
		rgb_modulation.R = r;
		Modulate = rgb_modulation;
	}

	public void CreateRecipe()
	{
		Text = "";
		List<LiquidData> target = new List<LiquidData>();
		for (int i = 0; i < controller.customerData.Final_drink_target.DrinkList.Count(); i++)
		{
			target.Add(controller.customerData.Final_drink_target.DrinkList[i]);
		}

		List<string> targetIngredientTypes = target.Select(x => x.LiquidName).Distinct().ToList();

		(target, targetIngredientTypes) = RandomizeRecipe(target, targetIngredientTypes);

		for (int i = 0; i < targetIngredientTypes.Count(); i++)
		{
			string liquidName = targetIngredientTypes[i];
			int targetAmount = target.Where(x => x.LiquidName == liquidName).Select(x => x.LiquidName).Count();

			string newRecipeLine = targetAmount + " drops of " + liquidName + System.Environment.NewLine;
			Text += newRecipeLine;
		}
	}

	private (List<LiquidData>, List<string>) RandomizeRecipe(List<LiquidData> recipe, List<string> ingredientNames)
	{
		int howManyToReplace = rand.Next(1, ingredientNames.Count());
		//GD.Print("Replacing " + howManyToReplace + " ingredients");
		int randomIngredientToRemove = 0;
		int randomIngredientToAdd = 0;
		int[] removedIngredientCount = new int[howManyToReplace];

		for (int i = 0; i < howManyToReplace; i++)
		{
			randomIngredientToRemove = rand.Next(ingredientNames.Count());
			int count = recipe.Count(x => x.LiquidName == ingredientNames[randomIngredientToRemove]);
			removedIngredientCount[i] = count;
			//GD.Print("Removed " + removedIngredientCount[i] + " " + ingredientNames[randomIngredientToRemove]);
			recipe = recipe.Where(x => x.LiquidName != ingredientNames[randomIngredientToRemove]).ToList();

			ingredientNames = ingredientNames.Where(x => x != ingredientNames[randomIngredientToRemove]).ToList();
		}

		for (int i = 0; i < howManyToReplace; i++)
		{
			randomIngredientToAdd = rand.Next(rand.Next(ingredientList.liquids.Length));
			if (ingredientNames.Contains(ingredientList.liquids[randomIngredientToAdd].LiquidName) == false)
				ingredientNames.Add(ingredientList.liquids[randomIngredientToAdd].LiquidName);
			//GD.Print("Added " + removedIngredientCount[i] + " " + ingredientList.liquids[randomIngredientToAdd].LiquidName);

			for (int ii = 0; ii < removedIngredientCount[i]; ii++)
			{
				recipe.Add(ingredientList.liquids[randomIngredientToAdd]);
			}
		}

		return (recipe, ingredientNames);
	}
}
