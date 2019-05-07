using Godot;
using System;

public class Event_chapter1_a1 : AnimationPlayer
{
	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile = string.Empty;

	[Export]
	private SpriteFrames raviaPortrait;

	[Export]
	private SpriteFrames neftaliPortrait;
	

	// ================================================================

	private void Resume()
	{
		GetParent<Event>().ResumeEvent();
	}

	// ================================================================

	public void Event_WalkUp()
	{
		Player.Walking = true;
		Player.MotionOverrideVec = new Vector2(0, -1);
		Player.MotionOverride = true;
		Player.WalkSpeedOverride = 140f;
	}


	public void Event_StopWalking()
	{
		Player.Walking = false;
		Player.MotionOverrideVec = new Vector2(0, 0);
	}


	public void Event_Dialogue1()
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, 0, "Ravia", "#2391ef",  raviaPortrait, signalConnection: this, signalMethod: "Resume", restoreMovement: false);//, "Neftali", "#ff0000", neftaliPortrait,  restoreMovement: false);
	}
}
