using Godot;
using System;

public class Event_opening_a1 : AnimationPlayer
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

	public void Event_Fadein()
	{
		Controller.Fade(true, false, 4.5f);
	}


	public void Event_WalkRight()
	{
		//Player.CurrentSpriteSet = Player.SpriteSet.PAPER;
		Player.Walking = true;
		Player.MotionOverride = true;
		Player.MotionOverrideVec = new Vector2(1, 0);
		Player.WalkSpeedOverride = 80f;
		Player.Face = Player.SpriteDirection.RIGHT;
		//Controller.CurrentMusic = music;
	}


	public void Event_StopRight()
	{
		GetParent<Event>().StopPlayer(Player.SpriteDirection.RIGHT);
	}


	public void Event_Dialogue()
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, 0, "Ravia", "#2391ef",  raviaPortrait, signalConnection: this, signalMethod: "Resume", restoreMovement: false);
	}
}
