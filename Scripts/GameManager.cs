using System;
using System.Linq;
using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
	[Export] GameRound[] gameRounds;
	[Export(PropertyHint.FilePath)] EventController eventController;
	[Export(PropertyHint.FilePath)] DrinkSubmissionArea drinkSubmissionArea;
	[Export(PropertyHint.FilePath)] Label timerLabel;
	[Export(PropertyHint.FilePath)] Timer startGameTimer;
	[Export(PropertyHint.FilePath)] public DrinkContainer glass;
	private bool isFullScreen;

	private GameRound currentRound;
	private int currentRoundIndex = 0;
	private int currentCustomerIndex = 0;

	[Export] public double allowedTimeToMixDrinks = 60;
	private double timer;
	private bool timerIsEnabled = false;

	public double[] customerScores;

	public override void _Ready()
	{
		eventController.Connect("EndOfIntroduction", new Callable(this, MethodName.StartCurrentCustomerDrinkMakingSection));
		eventController.Connect("EndOfCustomerSequence", new Callable(this, MethodName.EndCurrentCustomerInteraction));
		VisibleOnScreenNotifier2D glassVisible = glass.GetChild<VisibleOnScreenNotifier2D>(5);
		glassVisible.Connect("screen_exited", new Callable(this, MethodName.RespawnGlass));
		

		drinkSubmissionArea.OnDrinkSubmitted += SubmitDrinkToCustomer;
		timerLabel.Text = "";

		startGameTimer.Timeout += StartNextRound;
		customerScores = new double[gameRounds[currentRoundIndex].GetCustomerNumber()];
		isFullScreen = true;
	}

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("Fullscreen"))
    	{
			if (isFullScreen)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				isFullScreen = false;
			} else
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				isFullScreen = true;
			}	
    	}

		if (Input.IsActionJustPressed("Exit"))
		{
			GetTree().Quit();
		}
	}

	public void StartGame()
	{
		startGameTimer.Start();
	}
	
	private void RespawnGlass()
	{
		glass.GlobalPosition = new Vector2(-176, 180);
		glass.Freeze = true;
		glass.liquidContainer.liquids = new List<LiquidData>();
		glass.liquidContainer.currentVolume = 0;
		Godot.Collections.Array<Node> children = glass.liquidContainer.GetChildren();
		for (int i = 0; i < children.Count(); i++)
		{
			if (children[i] is Garnish)
			{
				children[i].QueueFree();
			}
		}

		glass.liquidContainer.UpdateLiquidShader();
	}

	private void StartNextRound()
	{
		currentRound = gameRounds[currentRoundIndex];

		StartNextCustomerInteraction();
	}

	private void EndCurrentRound()
	{
		currentRound = null;
		currentRoundIndex++;
		currentCustomerIndex = 0;
	}

	private void StartNextCustomerInteraction()
	{
		var currentCustomer = currentRound.GetCustomerAtIndex(currentCustomerIndex);

		//means we've run out of customers for this round, play the next round instead;
		if (currentCustomer == null)
		{
			//wrong place for this logic but out of time to figure out where to make it go
			eventController.ChangeGameState("FinalScore");
			return;
		}

		eventController.StartCustomerSequence(currentCustomer);
	}

	private void StartCurrentCustomerDrinkMakingSection()
	{
		GD.Print("Starting Current Customer Drink Making Section");
		timer = allowedTimeToMixDrinks;
		timerIsEnabled = true;
	}

	private void EndCurrentCustomerInteraction(double score)
	{
		GD.Print("Ending customer interaction");
		customerScores[currentCustomerIndex] = score;
		currentCustomerIndex++;

		timerIsEnabled = false;
		timerLabel.Text = "";
		StartNextCustomerInteraction();
	}

	private void UpdateTimerWithDelta(double delta)
	{
		timer = timer - delta;
		timerLabel.Text = $"Time Left: {Math.Round(timer)}s";
	}

	private void PlayerRunOutOfTime()
	{
		GD.Print("Player run out of time! " + timer);
		eventController.MoveToOutroAndScoringSequence(true, null);
		timerIsEnabled = false;
		timerLabel.Text = "";
	}

	public override void _Process(double delta)
	{
		if (!timerIsEnabled) return;

		UpdateTimerWithDelta(delta);

		if (timer > 0) return;

		PlayerRunOutOfTime();
		//time ran out, so maybe we should do some sort of override to give score of 0? handle later
		//EndCurrentCustomerInteraction();
	}

	public void SubmitDrinkToCustomer(DrinkContainer drink)
	{
		if (drink == null) return;
		
		if (eventController.gmstate == EventController.GameState.Gameplay)
		{
			DrinkContainer lastGlass = glass;
			eventController.MoveToOutroAndScoringSequence(false, lastGlass);
			timerIsEnabled = false;
			timerLabel.Text = "";
			RespawnGlass();
		}
	}
}

//Start customer sequence method

//when it sends signal start timer

//customer fades out signal to end round


//Game Round Resource
//Holds list of customers in order
//
