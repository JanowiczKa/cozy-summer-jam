using Godot;

[GlobalClass]
public partial class ChatBotDialogueBank : Resource
{
	[Export]
	public string[] IntroDialogue {get; set;}

	[Export]
	public string[] OutroDialogue {get; set;}

	[Export]
	public string FinalScore {get; set;}

	[Export]
	public string[] FinalScoreResult {get; set;}

	public ChatBotDialogueBank() : this(null, null, null, null) {}

	public ChatBotDialogueBank(string[] introDialogue, string[] outroDialogue, string finalScore, string[] finalScoreResult)
	{
		IntroDialogue = introDialogue;
		OutroDialogue = outroDialogue;
		FinalScore = finalScore;
		FinalScoreResult = finalScoreResult;
	}
}
