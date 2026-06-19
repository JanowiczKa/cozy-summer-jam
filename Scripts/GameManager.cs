using System;
using Godot;

public partial class GameManager : Node
{
	[Export] GameRound[] gameRounds;
	[Export(PropertyHint.FilePath)] EventController eventController;
	[Export(PropertyHint.FilePath)] DrinkSubmissionArea drinkSubmissionArea;
	[Export(PropertyHint.FilePath)] Label timerLabel;
	[Export(PropertyHint.FilePath)] Timer startGameTimer;

	private GameRound currentRound;
	private int currentRoundIndex = 0;
	private int currentCustomerIndex = 0;

	[Export] public double allowedTimeToMixDrinks = 40;
	private double timer;
	private bool timerIsEnabled = false;

	public override void _Ready()
	{
		eventController.Connect("EndOfIntroduction", new Callable(this, MethodName.StartCurrentCustomerDrinkMakingSection));
		eventController.Connect("EndOfCustomerSequence", new Callable(this, MethodName.EndCurrentCustomerInteraction));

		drinkSubmissionArea.OnDrinkSubmitted += SubmitDrinkToCustomer;
		timerLabel.Text = "";

		startGameTimer.Timeout += StartNextRound;
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
			EndCurrentRound();
			StartNextRound();
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

	private void EndCurrentCustomerInteraction()
	{
		GD.Print("Ending customer interaction");
		currentCustomerIndex++;

		StartNextCustomerInteraction();
		timerLabel.Text = "";
	}

	private void UpdateTimerWithDelta(double delta)
	{
		timer -= delta;
		timerLabel.Text = $"Time Left: {Math.Round(timer)}s";
	}

	public override void _Process(double delta)
	{
		if (!timerIsEnabled) return;

		UpdateTimerWithDelta(delta);

		if (timer < 0) return;

		//time ran out, so maybe we should do some sort of override to give score of 0? handle later
		//EndCurrentCustomerInteraction();
	}

	public void SubmitDrinkToCustomer(DrinkContainer drink)
	{
		//pass drink data
		//EndCurrentCustomerInteraction();
		eventController.MoveToOutroAndScoringSequence(false);

		GD.Print("Submitted drink!");
	}
}

//Start customer sequence method

//when it sends signal start timer

//customer fades out signal to end round


//Game Round Resource
//Holds list of customers in order
//
