using Godot;

[GlobalClass]
public partial class DialogAndExpression : Resource
{
	[Export]
	public string[] Dialog {get; set;}
	[Export]
	public string[] Expression {get; set;}

	public DialogAndExpression() : this(null, null) {}

	public DialogAndExpression(string[] dialog, string[] expression)
	{
		Dialog = dialog;
		Expression = expression;
	}

	public string GetDialogAtIndex(int i)
	{
		return Dialog[i];
	}

	public string GetExpressionAtIndex(int i)
	{
		return Expression[i];
	}
}
