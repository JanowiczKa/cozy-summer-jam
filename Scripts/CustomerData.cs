using Godot;

[GlobalClass]
public partial class CustomerData : Resource
{
	
	[Export]
	public Drink Final_drink_target {get; set;}
	
	[Export]
	public DialogAndExpression Intro_dialog_and_expressions {get; set;}

	[Export]
	public DialogAndExpression Gameplay_dialog_and_expressions {get; set;}
	 
	[Export]
	public DialogAndExpression Result_perfect {get; set;}

	[Export]
	public DialogAndExpression Result_mixed {get; set;}

	[Export]
	public DialogAndExpression Result_bad {get; set;}

	[Export]
	public DialogAndExpression Result_empty {get; set;}
	
	[Export]
	public DialogAndExpression Outro_dialog_and_expressions {get; set;}

	[Export]
	public Texture2D Base_customer_texture {get; set;}

	[Export]
	public SpriteFrames Customer_expression_textures {get; set;}

	public CustomerData() : this(null, null, null, null, null, null, null, null, null, null) {}

	public CustomerData(Drink drink_target, DialogAndExpression intro, 
	DialogAndExpression gameplay, DialogAndExpression resultPerfect, DialogAndExpression resultMixed, 
	DialogAndExpression resultBad, DialogAndExpression resultEmpty, DialogAndExpression outro, Texture2D base_tex, SpriteFrames frames)
	{
		Final_drink_target = drink_target;
		Intro_dialog_and_expressions = intro;
		Gameplay_dialog_and_expressions = gameplay;
		Result_perfect = resultPerfect;
		Result_mixed = resultMixed;
		Result_bad = resultBad;
		Result_empty = resultEmpty;
		Outro_dialog_and_expressions = outro;
		Base_customer_texture = base_tex;
		Customer_expression_textures = frames;
	}
}
