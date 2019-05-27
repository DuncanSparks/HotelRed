using Godot;
using System;

public class Event_chapter1_neftali : AnimationPlayer
{
	[Export]
	private SpriteFrames raviaFrames;

	[Export]
	private SpriteFrames neftaliFrames;

	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile = string.Empty;

	[Export]
	private NodePath neftaliInstance;

	[Export]
	private AudioStream neftaliMusic;

	// Refs
	private EventNPC neftaliNPC;

	// ================================================================

	public override void _Ready()
	{
		neftaliNPC = GetNode<EventNPC>(neftaliInstance);//GetNode<EventNPC>(neftaliInstance);
	}

	// ================================================================

	private void Resume()
	{
		GetParent<Event>().ResumeEvent();
	}

	// ================================================================

	public void Event_FadeMusic()
	{
		Controller.FadeMusic(0.5f);
	}


	public void Event_PlayNeftaliMusic()
	{
		Controller.PlayCharacterTheme(neftaliMusic);
	}


	public void Event_Dialogue(int set)
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, set, "Ravia", "#2391ef",  raviaFrames, "Neftali", "#ff0000", neftaliFrames, false, this, "Resume");
	}


	public void Event_RaviaTurn(int direction)
	{
		Player.Face = (Player.SpriteDirection)direction;
	}


	public void Event_NeftaliTurn(int direction)
	{
		neftaliNPC.Face = (EventNPC.SpriteDirection)direction;
	}


	public void Event_NeftaliWalk(float speed, int motionX, int motionY, int direction)
	{
		neftaliNPC.WalkSpeed = speed;
		neftaliNPC.Motion = new Vector2(motionX, motionY);
		neftaliNPC.Face = (EventNPC.SpriteDirection)direction;
		neftaliNPC.Walking = true;
	}


	public void Event_RaviaWalk(float speed, int motionX, int motionY, int direction)
	{
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


	public void Event_RaviaBubble(int type)
	{
		Controller.ShowBubble((Controller.BubbleType)type, Player.BubblePosition);
	}


	public void Event_NeftaliStop()
	{
		neftaliNPC.Motion = new Vector2(0, 0);
		neftaliNPC.WalkSpeed = 0f;
		neftaliNPC.Walking = false;
	}


	public void Event_BothWalkLeft()
	{
		Event_NeftaliWalk(180f, -1, 0, 2);
		Player.Walking = true;
		Player.MotionOverrideVec = new Vector2(-1, 0);
		Player.Face = Player.SpriteDirection.LEFT;
		Player.MotionOverride = true;
		Player.WalkSpeedOverride = 180f;
	}


	public void Event_SoulFadeIn()
	{
		Player.DepthControl = false;
		Player.Main.ZIndex = 1600;

		neftaliNPC.DepthControl = false;
		neftaliNPC.ZIndex = 1600;
	}


	public void Event_SoulFadeOut()
	{
		neftaliNPC.Hide();
	}

	/* public void Event_Dialogue1()
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, 0, "Ravia", "#2391ef",  raviaFrames, "Neftali", "#ff0000", neftaliFrames, false, this, "Resume");
	}


	public void Event_NeftaliWalkUp()
	{
		neftaliNPC.WalkSpeed = 140f;
		neftaliNPC.Motion = new Vector2(0, -1);
		neftaliNPC.Face = EventNPC.SpriteDirection.UP;
		neftaliNPC.Walking = true;
	}


	public void Event_RaviaSilence()
	{
		Controller.ShowBubble(Controller.BubbleType.SILENCE, Player.BubblePosition);
	}


	public void Event_NeftaliWalkLeft()
	{
		neftaliNPC.Motion = new Vector2(-1, 0);
		neftaliNPC.Face = EventNPC.SpriteDirection.LEFT;
	}


	public void Event_RaviaTurnUp()
	{
		Player.Face = Player.SpriteDirection.UP;
	}


	public void Event_NeftaliWalkDown()
	{
		neftaliNPC.Motion = new Vector2(0, 1);
		neftaliNPC.Face = EventNPC.SpriteDirection.DOWN;
	}


	public void Event_NeftaliStop()
	{
		neftaliNPC.Motion = new Vector2(0, 0);
		neftaliNPC.WalkSpeed = 0f;
		neftaliNPC.Walking = false;
	}


	public void Event_Dialogue2()
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, 1, "Ravia", "#2391ef",  raviaFrames, "Neftali", "#ff0000", neftaliFrames, false, this, "Resume");
	}


	public void Event_Dialogue3()
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, 2, "Ravia", "#2391ef",  raviaFrames, "Neftali", "#ff0000", neftaliFrames, false, this, "Resume");
	}


	public void Event_NeftaliTurnDown()
	{
		neftaliNPC.Face = EventNPC.SpriteDirection.DOWN;
	}


	public void Event_Dialogue4()
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, 3, "Ravia", "#2391ef",  raviaFrames, "Neftali", "#ff0000", neftaliFrames, false, this, "Resume");
	}


	public void Event_Ravia*/
}
