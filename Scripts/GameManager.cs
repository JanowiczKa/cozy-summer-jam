using System.ComponentModel.DataAnnotations.Schema;
using Godot;

public partial class GameManager : Node
{
	[Export] GameRound[] gameRounds;
	[Export(PropertyHint.FilePath)] EventController eventController;
	[Export(PropertyHint.FilePath)] DrinkSubmissionArea drinkSubmissionArea;
	[Export(PropertyHint.FilePath)] public DrinkContainer glass;

	private GameRound currentRound;
	private int currentRoundIndex = 0;
	private int currentCustomerIndex = 0;

	[Export] public double allowedTimeToMixDrinks = 40;
	private double timer;
	private bool timerIsEnabled = false;

	public override void _Ready()
	{
		StartNextRound();

		eventController.Connect("EndOfIntroduction", new Callable(this, MethodName.StartCurrentCustomerDrinkMakingSection));
		eventController.Connect("EndOfCustomerSequence", new Callable(this, MethodName.EndCurrentCustomerInteraction));
		VisibleOnScreenNotifier2D glassVisible = glass.GetChild<VisibleOnScreenNotifier2D>(5);
		glassVisible.Connect("screen_exited", new Callable(this, MethodName.RespawnGlass));
		

		drinkSubmissionArea.OnDrinkSubmitted += SubmitDrinkToCustomer;
	}

	private void RespawnGlass()
	{
		glass.QueueFree();
		var newGlass = GD.Load<PackedScene>("res://Scenes/DrinkGlass.tscn");
		var instance = newGlass.Instantiate();
		AddChild(instance);
		glass = (DrinkContainer)instance;
		glass.GlobalPosition = new Vector2(385, 408);
		VisibleOnScreenNotifier2D glassVisible = glass.GetChild<VisibleOnScreenNotifier2D>(5);
		glassVisible.Connect("screen_exited", new Callable(this, MethodName.RespawnGlass));
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
		timer = 0;
		timerIsEnabled = true;
	}

	private void EndCurrentCustomerInteraction()
	{
		currentCustomerIndex++;

		StartNextCustomerInteraction();
	}

	public override void _Process(double delta)
	{
		if (!timerIsEnabled) return;

		timer += delta;

		if (timer < allowedTimeToMixDrinks) return;

		//time ran out, so maybe we should do some sort of override to give score of 0? handle later
		//EndCurrentCustomerInteraction();
	}

	public void SubmitDrinkToCustomer(DrinkContainer drink)
	{
		//pass drink data
		//EndCurrentCustomerInteraction();
		
		if (eventController.gmstate == EventController.GameState.Gameplay)
		{
			DrinkContainer lastGlass = glass;
			eventController.MoveToOutroAndScoringSequence(false, lastGlass);
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
