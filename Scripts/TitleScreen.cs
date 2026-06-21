using Godot;
using System;

public partial class TitleScreen : Node2D
{

	[Export(PropertyHint.FilePath)] Area2D clickableArea;
	[Export(PropertyHint.FilePath)] Sprite2D blur;
	[Export(PropertyHint.FilePath)] GameManager manager;
	private bool hasBeenClicked;
	private float blurAmount;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		clickableArea.InputEvent += OnInputEvent;
		hasBeenClicked = false;
		blurAmount = (float)3.0;
		blur.SetInstanceShaderParameter("lod", blurAmount);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (hasBeenClicked == true)
		{
			Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, Modulate.A - (float)0.01);
			blurAmount -= (float)0.1;
			blur.SetInstanceShaderParameter("lod", blurAmount);
			if (Modulate.A <= (float)0.0)
			{
				manager.StartGame();
				QueueFree();
			}
		}
	}

	public void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{	
		var isMouseEvent = @event is InputEventMouseButton;
		
		if (!isMouseEvent) return;
		
		var mouseEvent = (InputEventMouseButton)@event;

		var pressed = mouseEvent.Pressed;
		var isLeftClick = mouseEvent.GetButtonIndex() == MouseButton.Left;

		if (isLeftClick && pressed && hasBeenClicked == false) 
			hasBeenClicked = true;
	}
}
