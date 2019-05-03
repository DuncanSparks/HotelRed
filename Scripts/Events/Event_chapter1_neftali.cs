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

	public void Event_Dialogue1()
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
}