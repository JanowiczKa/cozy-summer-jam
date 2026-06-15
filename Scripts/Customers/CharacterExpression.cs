using Godot;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

public partial class CharacterExpression : AnimatedSprite2D
{
	// Called when the node enters the s cene tree for the first time.
	public override void _Ready()
	{
		var parentNode = GetNode<Node2D>("../..");
		parentNode.Connect("SetExpression", new Callable(this, MethodName.ChangeExpression));
		parentNode.Connect("FadeOutAction", new Callable(this, MethodName.ResetExpression));
		parentNode.Connect("ExpressionClear", new Callable(this, MethodName.ResetExpression));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void ChangeExpression(string expression){
		
		switch(expression)
		{
			case "DefaultExp":
				Frame = 0;
				break;
			case "Mog":
				Frame = 1;
				break;
			default:
				Frame = 0;
				break;
		}
	}

	private void ResetExpression()
	{
		Frame = 0;
	}
}
