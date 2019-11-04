using Godot;
using System;

public class Event_generic : AnimationPlayer
{
	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile;

	[Export]
	private SpriteFrames raviaPortrait;
	
	[Export]
	private AudioStream music;

	// ================================================================

	public override void _Ready()
	{
		//Controller.PlayMusic(music);
		//Controller.CurrentMusic = music;
	}

	// ================================================================

	private void Resume()
	{
		GetParent<Event>().ResumeEvent();
	}

	// ================================================================

	public void Event_Dialogue(int set, string rightName, string rightColor, string rightPortrait)
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, set, "Ravia", "#2391ef",  raviaPortrait, rightClientName: rightName, rightClientColor: rightColor, rightClientPortrait: GD.Load<SpriteFrames>(rightPortrait), signalConnection: this, signalMethod: "Resume", restoreMovement: false);
	}
}
