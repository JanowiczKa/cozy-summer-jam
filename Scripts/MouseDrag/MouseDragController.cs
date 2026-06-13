using System;
using Godot;

public partial class MouseDragController : Node2D
{
	public MouseDrag currentDraggedObject;
	public float objectRotationSpeed = 6f;

	public Node2D holdingParentObject;

	public override void _Ready()
	{
		//shit for performance but only happens once on load, figure out better way if we have time, oof
		foreach (var node in GetTree().GetNodesInGroup("draggable")) //<= draggable is a group I've added to some scenes
		{
			//GD.Print(node.Name);
			
			var mouseDragScript = node as MouseDrag; //What a weird way to access the script attached to a node, defo doesn't feel safe

			//var mouseDragScript = node.GetScript().As<MouseDrag>(); <- straight up doesn't work

			//GD.Print(mouseDragScript);

			if (mouseDragScript == null) continue;

			mouseDragScript.OnBeginDragging += ObjectBeganDrag;
		}

		holdingParentObject = GetNode<Node2D>("HoldingObjectParent");

		GD.Print(holdingParentObject);
	}

	private void ObjectBeganDrag(MouseDrag mouseDrag)
	{
		currentDraggedObject = mouseDrag;

		holdingParentObject.GlobalPosition = GetGlobalMousePosition();

		currentDraggedObject.Reparent(holdingParentObject);

		GD.Print("Controller knows of drag");
	}

	public override void _Process(double delta)
	{
		var letGo = Input.IsActionJustReleased("Mouse1");

		if (currentDraggedObject == null || !letGo) return;

		currentDraggedObject.Reparent(this);
		currentDraggedObject.EndDragging();
		currentDraggedObject = null;
	}
	

	public override void _PhysicsProcess(double delta)
	{
		if (currentDraggedObject == null) return;

		if (Input.IsActionPressed("RotateHeldObjectLeft"))
		{
			holdingParentObject.GlobalRotationDegrees = holdingParentObject.GlobalRotationDegrees += objectRotationSpeed;
		}
		else if (Input.IsActionPressed("RotateHeldObjectRight"))
		{
			holdingParentObject.GlobalRotationDegrees = holdingParentObject.GlobalRotationDegrees -= objectRotationSpeed;
		}

		//Originally did this with physics impulses but was quite floaty so I gave up on that for the sake of time
		//but would really like to make that work because collision would work nicer

		var newPosition = GetGlobalMousePosition();

		holdingParentObject.GlobalPosition = newPosition;
	}
}
