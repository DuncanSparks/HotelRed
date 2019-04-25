using Godot;
using System;

public class NPC : KinematicBody2D
{
	[Export]
	private string npcName;

	[Export]
	private SpriteFrames npcSprite;

	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile;

	// Refs
	private AnimatedSprite spr;
	private Sprite interact;

	// ================================================================

    public override void _Ready()
    {
		spr = GetNode<AnimatedSprite>("Sprite");
		interact = GetNode<Sprite>("Interact");

		spr.Frames = npcSprite;
		interact.Hide();
    }

	// ================================================================

	private void InteractAreaEntered(Area2D area)
	{
		if (area.IsInGroup("PlayerSight"))
		{
			interact.Show();
		}
	}


	private void InteractAreaExited(Area2D area)
	{
		if (area.IsInGroup("PlayerSight"))
		{
			interact.Hide();
		}
	}
}
