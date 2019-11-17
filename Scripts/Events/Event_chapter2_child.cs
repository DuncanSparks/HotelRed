using Godot;
using System;

public class Event_chapter2_child : AnimationPlayer
{
	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile = string.Empty;

	[Export]
	private SpriteFrames raviaFrames;

	[Export]
	private SpriteFrames childFrames;

	// ================================================================

	private void Resume()
	{
		GetParent<Event>().ResumeEvent();
	}

	// ================================================================

	public void Event_RaviaBubble(int type)
	{
		Controller.ShowBubble((Controller.BubbleType)type, Player.BubblePosition);
	}


	public void Event_Dialogue(int set)
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, set, "Ravia", "#2391ef",  raviaFrames, "Little Girl", "#0a3898", childFrames, false, this, "Resume");
	}


	public void Event_DialogueSingle(int set)
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, set, "Ravia", "#2391ef", raviaFrames, restoreMovement: false, signalConnection: this, signalMethod: "Resume");
	}


	public void Event_RaviaWalk(float speed, int motionX, int motionY, int direction)
	{
		Player.CurrentSpriteSet = Player.SpriteSet.NORMAL;
		Player.Walking = true;
		Player.MotionOverrideVec = new Vector2(motionX, motionY);
		Player.Face = (Player.SpriteDirection)direction;
		Player.MotionOverride = true;
		Player.WalkSpeedOverride = speed;
	}


	public void Event_RaviaStop()
	{
		Player.Walking = false;
		Player.MotionOverrideVec = new Vector2(0, 0);
	}
}
