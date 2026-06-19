using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EventController : Node
{
	[Export(PropertyHint.FilePath)] Area2D characterInteractionArea;

	private enum GameState
	{
		Idle,
		Introduction,
		Gameplay,
		Outro,
		Result
	}
	private string[] expression_sequence;
	private string[] speech_sequence;
	private int sequence_index;
	private double total_time_elapsed;
	private bool waiting_for_text_to_finish;
	private GameState gmstate { get; set; }
	private Random rand;
	private CharacterSprite characterSprite;
	private Chatbot chatbot;
	private ChatBotDialogue chatbotDialogue;
	public double score;

	[Export]
	public CustomerData customerData;
	[Signal]
	private delegate void EndOfIntroductionEventHandler();
	[Signal]
	private delegate void EndOfCustomerSequenceEventHandler();
	[Signal]
	private delegate void ChangeExpressionEventHandler(string expression);
	[Signal]
	private delegate void StartNextDialogEventHandler(string speech);
	[Signal]
	private delegate void StartBounceAnimationEventHandler();
	[Signal]
	private delegate void StartDrinkingAnimationEventHandler();
	[Signal]
	private delegate void StartFadeInEventHandler();
	[Signal]
	private delegate void StartFadeOutEventHandler();
	[Signal]
	private delegate void ClearDialogAndExpressionEventHandler();

	public static EventController Instance;

    public override void _EnterTree()
    {
		Instance = this;
    }


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		expression_sequence = [];
		speech_sequence = [];
		sequence_index = 0;
		total_time_elapsed = 0.0;
		rand = new Random();
		waiting_for_text_to_finish = false;
		score = 0.0;
		characterSprite = GetNode<CharacterSprite>("../Characters/CharacterSprite");
		chatbot = GetNode<Chatbot>("../Chatbot");
		chatbotDialogue = GetNode<ChatBotDialogue>("../Chatbot/ChatBotDialogue");

		gmstate = GameState.Idle;
		var charactersNode = GetNode<Node2D>("../Characters");
		charactersNode.Connect("ControllerSequenceEnd", new Callable(this, MethodName.ChangeGameState));
		var spriteNode = GetNode<CharacterSprite>("../Characters/CharacterSprite");
		spriteNode.Connect("DrinkingFinished", new Callable(this, MethodName.ChangeGameState));
		var textNode = GetNode<RichTextLabel>("../Characters/SpeechBubble/BubbleText");
		textNode.Connect("RestartGameplayTextTimer", new Callable(this, MethodName.ResetTimerForText));

		characterInteractionArea.InputEvent += OnInputEvent;
	}

	public void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{	
		var isMouseEvent = @event is InputEventMouseButton;
		
		if (!isMouseEvent) return;
		
		var mouseEvent = (InputEventMouseButton)@event;

		var pressed = mouseEvent.Pressed;
		var isLeftClick = mouseEvent.GetButtonIndex() == MouseButton.Left;
		
		if (isLeftClick && pressed && !MouseDragController.Instance.IsHoldingObject()) 
			CharacterDialogueUpdate();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (chatbot.isExtended != true)
		{
			total_time_elapsed += delta;
		}

		if (gmstate == GameState.Gameplay && total_time_elapsed >= 10.0 && waiting_for_text_to_finish == false)
		{
			LoopMessagesWhenMakingDrink();
		}
	}

	private void CharacterDialogueUpdate()
	{
		if (gmstate == GameState.Introduction || gmstate == GameState.Result || gmstate == GameState.Outro)
		{
			// Change game states if the at the end of a dialog sequence
			if (sequence_index >= expression_sequence.Length || sequence_index >= speech_sequence.Length)
			{
				switch(gmstate)
				{
					case GameState.Introduction:
						if (chatbotDialogue.dlgMode == ChatBotDialogue.DialogueMode.Default)
						{
							EmitSignal(SignalName.ClearDialogAndExpression);
							EmitSignal(SignalName.StartBounceAnimation);
							chatbotDialogue.PlayIntro(customerData.Final_drink_target.DrinkName);
						}
						break;
					case GameState.Outro:
						if (characterSprite.is_animated)
							break;
						EmitSignal(SignalName.StartDrinkingAnimation);
						EmitSignal(SignalName.ClearDialogAndExpression);
						break;
					case GameState.Result:
						EmitSignal(SignalName.StartFadeOut);

						chatbotDialogue.PlayOutro(score);

						EmitSignal(SignalName.ClearDialogAndExpression);
						break;
				}
				return;
			}

			// If not at the end of a dialog sequence
			if (characterSprite.is_animated != true)
			{
				TriggerNextDialogLine();
			}
				
		}
	}

	// public override void _Input(InputEvent @event)
	// {
	// 	// Player input for Introduction, Outro and Result dialog sequences
	// 	if (@event.IsActionPressed("Space") && (gmstate == GameState.Introduction || gmstate == GameState.Result || gmstate == GameState.Outro))
	// 	{
	// 		// Change game states if the at the end of a dialog sequence
	// 		if (sequence_index >= expression_sequence.Length || sequence_index >= speech_sequence.Length)
	// 		{
	// 			switch(gmstate)
	// 			{
	// 				case GameState.Introduction:
	// 					if (chatbotDialogue.dlgMode == ChatBotDialogue.DialogueMode.Default)
	// 					{
	// 						EmitSignal(SignalName.ClearDialogAndExpression);
	// 						EmitSignal(SignalName.StartBounceAnimation);
	// 						chatbotDialogue.PlayIntro(customerData.Final_drink_target.DrinkName);
	// 					}
	// 					break;
	// 				case GameState.Outro:
	// 					if (characterSprite.is_animated)
	// 						break;
	// 					EmitSignal(SignalName.StartDrinkingAnimation);
	// 					EmitSignal(SignalName.ClearDialogAndExpression);
	// 					break;
	// 				case GameState.Result:
	// 					EmitSignal(SignalName.StartFadeOut);

	// 					chatbotDialogue.PlayOutro(score);

	// 					EmitSignal(SignalName.ClearDialogAndExpression);
	// 					break;
	// 			}
	// 			return;
	// 		}

	// 		// If not at the end of a dialog sequence
	// 		if (characterSprite.is_animated != true)
	// 		{
	// 			TriggerNextDialogLine();
	// 		}
				
	// 	}

	// 	// // testing purposes only
	// 	// if (@event.IsActionPressed("FadeOut"))
	// 	// {
	// 	// 	MoveToOutroAndScoringSequence(true);
	// 	// }

	// 	// // testing purposes only
	// 	// if (@event.IsActionPressed("FadeIn"))
	// 	// {
	// 	// 	StartCustomerSequence(GD.Load<CustomerData>("res://Resources/Customers/Sans/SansData.tres"));
	// 	// }
	// }

	public void StartGameplaySequence()
	{
		ChangeGameState("Gameplay");

		// Signals the game manager to start the gameplay timer
		EmitSignal(SignalName.EndOfIntroduction);
	}

	// Loads a new customer resource and fades the customer in, starting the entire sequence
	public void StartCustomerSequence(CustomerData customer)
	{
		GD.Print("Starting customer sequence");
		customerData = customer;
		CharacterExpression expressionNode = GetNode<CharacterExpression>("../Characters/CharacterSprite/CharacterExpression");
		GD.Print(expressionNode.Name);
		expressionNode.SpriteFrames = customer.Customer_expression_textures;
		GD.Print("Emitting signal!");
		EmitSignal(SignalName.StartFadeIn);
	}

	// To be called when the gameplay timer ends or when the player passes the finished drink to the customer;
	// Starts the dialog leading into the score
	public void MoveToOutroAndScoringSequence(bool out_of_time)
	{
		// Check if the player ran out of time
		if (out_of_time == true)
		{
			ChangeGameState("OutOfTime");
			return;
		}
		// Check if drink is empty
		DrinkContainer container = GetNode<DrinkContainer>("../DrinkGlass");
		List<LiquidData> drink = container.liquidContainer.liquids;
		if (drink.Count() == 0)
		{
			ChangeGameState("Result");
			return;
		}
		ChangeGameState("Outro");
		TriggerNextDialogLine();
	}

	private void ResetTimerForText()
	{
		total_time_elapsed = 0.0;
		waiting_for_text_to_finish = false;
	}

	private void TriggerNextDialogLine()
	{
		EmitSignal(SignalName.ChangeExpression, expression_sequence[sequence_index]);
		EmitSignal(SignalName.StartNextDialog, speech_sequence[sequence_index]);
		// Do not play bounce animation if the text is set to ""
		if (expression_sequence[sequence_index] != "")
			EmitSignal(SignalName.StartBounceAnimation);
		sequence_index += 1;
	}

	private void LoopMessagesWhenMakingDrink()
	{
		sequence_index = rand.Next(expression_sequence.Length);
		EmitSignal(SignalName.ChangeExpression, expression_sequence[sequence_index]);
		EmitSignal(SignalName.StartNextDialog, speech_sequence[sequence_index]);
		// Do not play bounce animation if the text is set to ""
		if (expression_sequence[sequence_index] != "")
			EmitSignal(SignalName.StartBounceAnimation);
		waiting_for_text_to_finish = true;
	}
	
	// Huge fucking method, I know, but works pretty damn well.
	// It extracts the names of all unique ingredients from the LiquidData list
	// and iterates over each one, comparing the player created drink to
	// the recipe. As it does so, it calculates the difference between
	// the volume of an ingredient in the recipe and the player made drink
	// to then later calculate the score based off of the result.
	// If a player's drink contains an ingredient the recipe doesn't,
	// the difference equals 0 (since 0 divided by anything is 0) and
	// vice versa.
	private double VerifyDrinkAndScore()
	{
		DrinkContainer container = GetNode<DrinkContainer>("../DrinkGlass");
		double score = 0.0;
		List<LiquidData> target = new List<LiquidData>();
		List<LiquidData> drink = container.liquidContainer.liquids;
		for (int i = 0; i < customerData.Final_drink_target.DrinkList.Length; i++)
		{
			target.Add(customerData.Final_drink_target.DrinkList[i]);
		}
		List<string> targetIngredientTypes = target.Select(x => x.LiquidName).Distinct().ToList();
		List<string> playerDrinkIngredientTypes = drink.Select(x => x.LiquidName).Distinct().ToList();

		if (playerDrinkIngredientTypes.Count() > 0)
		{
			for (int i = 0; i < playerDrinkIngredientTypes.Count(); i++)
			{
				string liquidName = playerDrinkIngredientTypes[i];
				int presentAmount = drink.Where(x => x.LiquidName == liquidName).Select(x => x.LiquidName).Count();
				int targetAmount = target.Where(x => x.LiquidName == liquidName).Select(x => x.LiquidName).Count();
				
				double result = 0.0;

				if (presentAmount <= targetAmount)
				{
					result = 100 / targetIngredientTypes.Count() * ((double)presentAmount / targetAmount);
				}
				else
				{
					result = 100 / targetIngredientTypes.Count() * ((double)targetAmount / presentAmount);
				}
				score += result;
			}
		}
		return score;
	}

	private void DetermineResult(double score)
	{
		DrinkContainer container = GetNode<DrinkContainer>("../DrinkGlass");
		List<LiquidData> drink = container.liquidContainer.liquids;
		if (drink.Count() == 0)
		{
			speech_sequence = customerData.Result_empty.Dialog;
			expression_sequence = customerData.Result_empty.Expression;
			return;
		}

		if (score >= 95)
		{
			speech_sequence = customerData.Result_perfect.Dialog;
			expression_sequence = customerData.Result_perfect.Expression;
		} else if (score >= 50)
		{
			speech_sequence = customerData.Result_mixed.Dialog;
			expression_sequence = customerData.Result_mixed.Expression;
		}
		else
		{
			speech_sequence = customerData.Result_bad.Dialog;
			expression_sequence = customerData.Result_bad.Expression;
		}
	}
	
	public void ChangeGameState(string sequence)
	{
		switch (sequence)
		{
			case "Introduction":
				sequence_index = 0;
				gmstate = GameState.Introduction;
				expression_sequence = customerData.Intro_dialog_and_expressions.Expression;
				speech_sequence = customerData.Intro_dialog_and_expressions.Dialog;
				TriggerNextDialogLine();
				break;
			case "Gameplay":
				gmstate = GameState.Gameplay;
				speech_sequence = customerData.Gameplay_dialog_and_expressions.Dialog;
				expression_sequence = customerData.Gameplay_dialog_and_expressions.Expression;
				total_time_elapsed = 0.0;
				break;
			case "Outro":
				gmstate = GameState.Outro;
				speech_sequence = customerData.Outro_dialog_and_expressions.Dialog;
				expression_sequence = customerData.Outro_dialog_and_expressions.Expression;
				sequence_index = 0;
				break;
			case "Result":
				sequence_index = 0;
				gmstate = GameState.Result;
				score = VerifyDrinkAndScore();
				DetermineResult(score);
				GD.Print(score);
				sequence_index = 0;
				TriggerNextDialogLine();
				break;
			case "End":
				gmstate = GameState.Idle;
				sequence_index = 0;
				score = 0.0;

				// Signals the game manager that the entire customer sequence has finished
				EmitSignal(SignalName.EndOfCustomerSequence);
				break;
			case "OutOfTime":
				sequence_index = 0;
				gmstate = GameState.Result;
				speech_sequence = customerData.Result_out_of_time.Dialog;
				expression_sequence = customerData.Result_out_of_time.Expression;
				sequence_index = 0;
				TriggerNextDialogLine();
				break;
		}
	}

}
