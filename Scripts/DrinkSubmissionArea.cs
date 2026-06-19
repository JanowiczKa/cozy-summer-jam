using Godot;
using System;

//this is actually really shitty design and will break if we have the player putting multiple glasses on here, but we'll restrict the player to one glass anyway so it should be alright at least for now
public partial class DrinkSubmissionArea : Node2D
{
	[Export(PropertyHint.FilePath)] public Area2D drinkSubmissionArea;

	[Export] public double timeToSubmit;

	private double timer;
	private DrinkContainer currentDrinkSubmitting;

	public Action<DrinkContainer> OnDrinkSubmitted;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		drinkSubmissionArea.BodyEntered += TryStartSubmissionTimer;
		drinkSubmissionArea.BodyExited += TryStopSubmissionTimer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (currentDrinkSubmitting == null) return;

		timer += delta;

		if (timer < timeToSubmit) return;

		SubmitDrink();
	}

	private void SubmitDrink()
	{
		OnDrinkSubmitted?.Invoke(currentDrinkSubmitting);
		currentDrinkSubmitting = null;
	}

	private void TryStartSubmissionTimer(Node2D body)
	{
		if (body is not DrinkContainer) return;

		currentDrinkSubmitting = body as DrinkContainer;
		timer = 0;
	}

	private void TryStopSubmissionTimer(Node2D body)
	{
		if (body != currentDrinkSubmitting) return; //technically can't happen since we'll only let the player have one glass at a time but better safe than sorry I suppose

		currentDrinkSubmitting = null;
	}
}
