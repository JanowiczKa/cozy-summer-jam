using Godot;
using System;

public partial class ChatBotDialogue : RichTextLabel
{
	[Export]
	public ChatBotDialogueBank dialogueBank;
	[Export]
	public bool autoExtend;
	public enum DialogueMode
	{
		Intro,
		Result,
		Thinking,
		Awaiting,
		Gameplay,
		Default
	}

	public DialogueMode dlgMode;
	private double total_time_elapsed;

	private string textTemp;
	private int thinkingCounter;
	private Color rgb_modulation;
	private ChatBotAudio audio;
	private bool is_player_scored;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		total_time_elapsed = 0.0;
		Text = "Awaiting orders...";
		VisibleCharacters = Text.Length;
		dlgMode = DialogueMode.Default;
		is_player_scored = false;
		textTemp = "";
		thinkingCounter = 0;
		rgb_modulation = new Color((float)0.0, (float)0.0, (float)0.0, (float)0.0);
		audio = GetNode<ChatBotAudio>("../ChatBotAudio");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		total_time_elapsed += delta;

		switch(dlgMode)
		{
			case DialogueMode.Intro:
				if (total_time_elapsed >= 0.05 && VisibleCharacters < Text.Length)
				{
					VisibleCharacters += 1;
					total_time_elapsed = 0.0;
					string blank_space_check = Text;

					if (VisibleCharacters == Text.Length)
					{
						dlgMode = DialogueMode.Awaiting;
						break;
					}

					if (blank_space_check[VisibleCharacters] != ' ' && blank_space_check[VisibleCharacters] != '.')
					{
						audio.PlayVoiceSE();
					}
				}
				break;
			case DialogueMode.Result:
				if (total_time_elapsed >= 0.05 && VisibleCharacters < Text.Length)
				{
					VisibleCharacters += 1;
					total_time_elapsed = 0.0;
					string blank_space_check = Text;

					if (VisibleCharacters == Text.Length)
					{
						dlgMode = DialogueMode.Awaiting;
						break;
					}

					if (blank_space_check[VisibleCharacters] != ' ' && blank_space_check[VisibleCharacters] != '.')
					{
						audio.PlayVoiceSE();
					}
				}
				break;
			case DialogueMode.Thinking:
				AnimateThinking(total_time_elapsed);
				break;
			case DialogueMode.Awaiting:
				if (total_time_elapsed >= 1.0 && is_player_scored == false)
				{
					dlgMode = DialogueMode.Gameplay;
					EventController controller = GetNode<EventController>("/root/BarScene/EventController");
					controller.StartGameplaySequence();
					Chatbot bot = GetParent<Chatbot>();
					bot.AnimationStart();
					ChatBotRecipe recipe = GetNode<ChatBotRecipe>("../ChatBotRecipe");
					recipe.CreateRecipe();
					Text = "Recipe ready!";
				} else if (total_time_elapsed >= 1.0 && is_player_scored == true)
				{
					is_player_scored = false;
					dlgMode = DialogueMode.Default;
					Text = "Awaiting orders...";
					EventController controller = GetNode<EventController>("/root/BarScene/EventController");
					controller.ChangeGameState("End");
				}
				break;
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

	public void PlayIntro(string drinkName)
	{
		string dialogueAndDrink = Tr(dialogueBank.IntroDialogue[0]).Replace("{drink}", drinkName);
		textTemp = dialogueAndDrink;
		Text = "Thinking...";
		VisibleCharacters = 8;
		dlgMode = DialogueMode.Thinking;
	}

	public void PlayOutro(double score)
	{
		string dialogueAndScore = "";

		if (score >= 85)
		{
			dialogueAndScore = Tr(dialogueBank.OutroDialogue[0]).Replace("{score}", score.ToString());
		} else if (score >= 50)
		{
			dialogueAndScore = Tr(dialogueBank.OutroDialogue[1]).Replace("{score}", score.ToString());
		}
		else if (score > 0.0)
		{
			dialogueAndScore = Tr(dialogueBank.OutroDialogue[2]).Replace("{score}", score.ToString());
		} else
		{
			dialogueAndScore = Tr(dialogueBank.OutroDialogue[3]).Replace("{score}", score.ToString());
		}
		textTemp = dialogueAndScore;
		Text = "Thinking...";
		VisibleCharacters = 8;
		is_player_scored = true;
		dlgMode = DialogueMode.Thinking;
	}

	private void AnimateThinking(double time)
	{
		if (total_time_elapsed >= 0.5 && VisibleCharacters < Text.Length)
		{
			VisibleCharacters += 1;
			total_time_elapsed = 0.0;
		} else if (total_time_elapsed >= 0.5 && VisibleCharacters == Text.Length)
		{
			GD.Print(Text.Length + " " + VisibleCharacters);
			VisibleCharacters = 8;
			thinkingCounter += 1;

			if (thinkingCounter >= 2)
			{
				thinkingCounter = 0;
				if (is_player_scored == false)
				{
					dlgMode = DialogueMode.Intro;
				}
				else
				{
					dlgMode = DialogueMode.Result;
				}
				VisibleCharacters = 0;
				Text = textTemp;
				textTemp = "";
			}
		}
	}
}
