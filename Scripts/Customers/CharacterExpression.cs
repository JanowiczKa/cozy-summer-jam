using Godot;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

public partial class CharacterExpression : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parentCharacter = GetParent<Sprite2D>();
		parentCharacter.Connect("NextLine", new Callable(this, MethodName.ChangeExpression));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void ChangeExpression(string expressionName){
		
		switch(expressionName)
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
}
