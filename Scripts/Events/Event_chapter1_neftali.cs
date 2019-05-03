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
		neftaliNPC = GetParent<Event>().GetParent<EventNPC>();//GetNode<EventNPC>(neftaliInstance);
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
		GD.Print("TEST");
		neftaliNPC.WalkSpeed = 180f;
		neftaliNPC.Motion = new Vector2(0, -1);
		neftaliNPC.Walking = true;
	}
}