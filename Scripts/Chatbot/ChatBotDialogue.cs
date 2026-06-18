using Godot;
using System;

public partial class ChatBotDialogue : RichTextLabel
{
	[Export]
	public ChatBotDialogueBank dialogueBank;
	[Export]
	public bool autoExtend;
	enum DialogueMode
	{
		Intro,
		Result,
		Thinking,
		Awaiting,
		Gameplay,
		Default
	}

	DialogueMode dlgMode;
	private double total_time_elapsed;

	private string textTemp;
	private int thinkingCounter;
	private Color rgb_modulation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		total_time_elapsed = 0.0;
		Text = "Awaiting orders...";
		VisibleCharacters = Text.Length;
		dlgMode = DialogueMode.Default;
		textTemp = "";
		thinkingCounter = 0;
		rgb_modulation = new Color((float)0.0, (float)0.0, (float)0.0, (float)0.0);
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
					if (VisibleCharacters == Text.Length)
					{
						dlgMode = DialogueMode.Awaiting;
					}
				}
				break;
			case DialogueMode.Thinking:
				AnimateThinking(total_time_elapsed);
				break;
			case DialogueMode.Awaiting:
				if (total_time_elapsed >= 1.0)
				{
					dlgMode = DialogueMode.Gameplay;
					EventController controller = GetNode<EventController>("/root/BarScene/EventController");
					controller.StartGameplaySequence();
					Chatbot bot = GetParent<Chatbot>();
					bot.AnimationStart();
					ChatBotRecipe recipe = GetNode<ChatBotRecipe>("../ChatBotRecipe");
					recipe.CreateRecipe();
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
				dlgMode = DialogueMode.Intro;
				VisibleCharacters = 0;
				Text = textTemp;
				textTemp = "";
			}
		}
	}
}
