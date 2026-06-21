using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ChatBotRecipe : RichTextLabel
{
	[Export]
	public ListOfIngredients ingredientList;
	private EventController controller;
	private GameManager manager;
	private Random rand;
	private Color rgb_modulation;
	public bool final_score, end_final_score;
	private double total_time_elapsed;
	private ChatBotAudio audio;
	private ChatBotDialogue dialogue;
	public double score;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		controller = GetNode<EventController>("/root/BarScene/EventController");
		manager = GetNode<GameManager>("/root/BarScene/GameManager");
		Text = "";
		rand = new Random();
		rgb_modulation = new Color((float)0.0, (float)0.0, (float)0.0, (float)0.0);
		Modulate = rgb_modulation;
		final_score = false;
		end_final_score = false;
		total_time_elapsed = 0.0;
		score = 0.0;
		audio = GetNode<ChatBotAudio>("../ChatBotAudio");
		dialogue = GetNode<ChatBotDialogue>("../ChatBotDialogue");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		total_time_elapsed += delta;

		if (total_time_elapsed >= 0.05 && final_score == true && end_final_score == false)
		{
			VisibleCharacters += 1;
			total_time_elapsed = 0.0;
			string blank_space_check = Text;

			if (VisibleCharacters == Text.Length)
			{
				end_final_score = true;
				dialogue.is_player_scored_finally = true;
				return;
			}

			if (blank_space_check[VisibleCharacters] != ' ' || blank_space_check[VisibleCharacters] != '.')
			{
				audio.PlayVoiceSE();
			}
		}
	}

	public void ChangeOpacity(float a, float b, float g, float r)
	{
		rgb_modulation.A = a;
		rgb_modulation.B = b;
		rgb_modulation.G = g;
		rgb_modulation.R = r;
		Modulate = rgb_modulation;
	}

	public void SetFinalScoreDisplay()
	{
		final_score = true;
		Text = "";
		VisibleCharacters = 0;
		for (int i = 0; i < manager.customerScores.Length; i++)
		{
			Text += "Customer #" + (i+1) + " " + manager.customerScores[i] + "%" + System.Environment.NewLine;
			score += manager.customerScores[i];
		}

		score = score / manager.customerScores.Length;
		Text += "Final score: " + score + "%";
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

			string newRecipeLine = targetAmount + " " + liquidName + System.Environment.NewLine;
			Text += newRecipeLine;
		}
	}

	private (List<LiquidData>, List<string>) RandomizeRecipe(List<LiquidData> recipe, List<string> ingredientNames)
	{
		int howManyToReplace = rand.Next(1, ingredientNames.Count());
		int randomIngredientToRemove = 0;
		int randomIngredientToAdd = 0;
		int[] removedIngredientCount = new int[howManyToReplace];

		for (int i = 0; i < howManyToReplace; i++)
		{
			randomIngredientToRemove = rand.Next(ingredientNames.Count());
			int count = recipe.Count(x => x.LiquidName == ingredientNames[randomIngredientToRemove]);
			removedIngredientCount[i] = count;

			recipe = recipe.Where(x => x.LiquidName != ingredientNames[randomIngredientToRemove]).ToList();

			ingredientNames = ingredientNames.Where(x => x != ingredientNames[randomIngredientToRemove]).ToList();
		}

		for (int i = 0; i < howManyToReplace; i++)
		{
			randomIngredientToAdd = rand.Next(rand.Next(ingredientList.liquids.Length));
			if (ingredientNames.Contains(ingredientList.liquids[randomIngredientToAdd].LiquidName) == false)
				ingredientNames.Add(ingredientList.liquids[randomIngredientToAdd].LiquidName);

			for (int ii = 0; ii < removedIngredientCount[i]; ii++)
			{
				recipe.Add(ingredientList.liquids[randomIngredientToAdd]);
			}
		}

		return (recipe, ingredientNames);
	}
}
