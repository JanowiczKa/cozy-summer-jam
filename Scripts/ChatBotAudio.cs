using Godot;
using System;

public partial class ChatBotAudio : AudioStreamPlayer
{
	private AudioStream se_1, se_2, se_3;
	private AudioStream[] se_list;
	private Random rand;
	 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rand = new Random();

		se_1 = GD.Load<AudioStream>("res://Audio/Chatbot/mixr_dial1.wav");
		se_2 = GD.Load<AudioStream>("res://Audio/Chatbot/mixr_dial2.wav");
		se_3 = GD.Load<AudioStream>("res://Audio/Chatbot/mixr_dial3.wav");

		se_list = [se_1, se_2, se_3];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PlayVoiceSE()
	{
		Stream = se_list[rand.Next(2)];
		Play((float)0.0);
	}
}
