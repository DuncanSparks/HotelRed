using Godot;
using System;

public class Event_opening_a2 : AnimationPlayer
{
	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile = string.Empty;

	[Export]
	private SpriteFrames raviaPortrait;

	[Export(PropertyHint.File, "*.tscn")]
	private string targetScene = string.Empty;
	
	[Export]
	AudioStream exclamation;
	
	[Export]
	AudioStream music;

	// ================================================================

	private void Resume()
	{
		GetParent<Event>().ResumeEvent();
	}

	// ================================================================

	public void Event_TurnUp()
	{
		Controller.StopMusic();
		Player.Face = Player.SpriteDirection.UP;
	}


	public void Event_Exclamation()
	{
		Controller.PlaySoundBurst(exclamation);
		Controller.ShowBubble(Controller.BubbleType.EXCLAMATION, Player.BubblePosition);
	}


	public void Event_WalkUp1()
	{	
		Controller.PlaySoundBurst(music, 1.5f);
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
		Controller.Dialogue(dialogueFile, 0, "Ravia", "#2391ef",  raviaPortrait, signalConnection: this, signalMethod: "Resume", restoreMovement: false);
	}


	public void Event_WalkUp2()
	{
		Player.Walking = true;
		Player.MotionOverrideVec = new Vector2(0, -1);
		Player.MotionOverride = true;
		Player.WalkSpeedOverride = 60f;
	}


	public void Event_Dialogue2()
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, 1, "Ravia", "#2391ef",  raviaPortrait, signalConnection: this, signalMethod: "Resume", restoreMovement: false);
	}


	public void Event_Transition()
	{
		Player.State = Player.ST.MOVE;
		Controller.SceneGoto(targetScene);
		Player.Main.Position = new Vector2(552, 652);
	}
}
