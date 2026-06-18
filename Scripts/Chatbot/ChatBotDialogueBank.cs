using Godot;

[GlobalClass]
public partial class ChatBotDialogueBank : Resource
{
	[Export]
	public string[] IntroDialogue {get; set;}

	[Export]
	public string[] OutroDialogue {get; set;}

	public ChatBotDialogueBank() : this(null, null) {}

	public ChatBotDialogueBank(string[] introDialogue, string[] outroDialogue)
	{
		IntroDialogue = introDialogue;
		OutroDialogue = outroDialogue;
	}
}
