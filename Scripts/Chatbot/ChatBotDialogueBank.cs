using Godot;

[GlobalClass]
public partial class ChatBotDialogueBank : Resource
{
    [Export]
    public string[] IntroDialogue {get; set;}

    public ChatBotDialogueBank() : this(null) {}

    public ChatBotDialogueBank(string[] introDialogue)
    {
        IntroDialogue = introDialogue;
    }
}