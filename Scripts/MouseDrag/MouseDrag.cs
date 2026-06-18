using System;
using Godot;

public partial class MouseDrag : RigidBody2D
{
	[Export(PropertyHint.File)] public Area2D optionalDraggableAreaCollider;

	public float mouseFollowSpeed = 30f;
	public float objectRotationSpeed = 10f;

	private bool isHeld = false;

	private Vector2 mouseClickOffset;

	public Action<MouseDrag> OnBeginDragging;

	public override void _Ready()
	{
		MouseDragController.Instance.RegisterMouseDrag(this);

		//GD.Print("I've been added to the tree");
		//These 2 are needed for the object to interract correctly with physics when picked up
		FreezeMode = FreezeModeEnum.Kinematic;
		InputPickable = true;

        //calls the OnInputEvent method when the collider detects user input overlapping it
		if (optionalDraggableAreaCollider == null)
		{
			InputEvent += OnInputEvent;
		}
		else
		{
			optionalDraggableAreaCollider.InputEvent += OnInputEvent;
		}
	}


	public void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{	
		var isMouseEvent = @event is InputEventMouseButton;
		
		if (!isMouseEvent) return;
		
		var mouseEvent = (InputEventMouseButton)@event;

		var pressed = mouseEvent.Pressed;
		var isLeftClick = mouseEvent.GetButtonIndex() == MouseButton.Left;
		
		if (isLeftClick && pressed) 
			BeginDragging();
	}

	public void BeginDragging()
	{
		GD.Print("Began dragging");

		mouseClickOffset = GlobalPosition - GetGlobalMousePosition();
		isHeld = true;
		Freeze = true;

		OnBeginDragging?.Invoke(this);
	}

	public void EndDragging()
	{
		isHeld = false;
		Freeze = false;
	}
}
