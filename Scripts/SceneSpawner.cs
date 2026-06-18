using Godot;
using System;

public partial class SceneSpawner : Node2D
{
	[Export] public Area2D clickableArea;

	[Export] PackedScene spawnableScene;

	public override void _Ready()
	{
		clickableArea.InputEvent += OnInputEvent;
	}

	public void OnInputEvent(Node viewport, InputEvent @event, long shape_idx)
	{	
		var isMouseEvent = @event is InputEventMouseButton;
		
		if (!isMouseEvent) return;
		
		var mouseEvent = (InputEventMouseButton)@event;

		var pressed = mouseEvent.Pressed;
		var isLeftClick = mouseEvent.GetButtonIndex() == MouseButton.Left;
		
		if (isLeftClick && pressed) 
			InstanciateSceneAtMousePosition();
	}

	public void InstanciateSceneAtMousePosition()
	{
		var instance = spawnableScene.Instantiate<Node2D>();

		instance.GlobalPosition = GetGlobalMousePosition();

		GetTree().Root.AddChild(instance);

		if (instance is MouseDrag)
		{
			var intanceAsMouseDrag = instance as MouseDrag;

			intanceAsMouseDrag.BeginDragging();
		}
	}
}


// using Godot;
// using System;

// public partial class DropletSpawner : Node2D
// {
// 	public static DropletSpawner Instance { get; private set; }

// 	[Export] PackedScene dropletScene;
	
// 	public override void _Ready()
//     {
//         Instance = this;
//     }

// 	//Still need to add it to the tree and change positions etcs manually, but now we only need one palce to have the packed scene
// 	public Droplet GetDropletInstance(LiquidData liquid)
// 	{
// 		var instance = dropletScene.Instantiate<Droplet>();

// 		instance.Setup(liquid);

// 		return instance;
// 	}
// }
