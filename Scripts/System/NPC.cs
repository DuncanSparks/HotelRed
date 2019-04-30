using Godot;
using System;

public class NPC : KinematicBody2D
{
	[Export]
	private string npcName = string.Empty;

	[Export]
	private Color npcColor = new Color(1, 1, 1);

	[Export]
	private SpriteFrames npcSprite;

	[Export]
	private SpriteFrames npcPortrait;

	[Export]
	private int maxDialogueSet;

	[Export]
	private bool autoAdvanceSet;

	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile = string.Empty;

	private int dialogueSet = 0;
	private string npcColorStr;
	private SpriteFrames raviaPortrait = GD.Load<SpriteFrames>("res://Resources/Portrait Sets/Portraits_Ravia.tres");

	// Refs
	private AnimatedSprite spr;
	private Sprite interact;

	// ================================================================

    public override void _Ready()
    {
		spr = GetNode<AnimatedSprite>("Sprite");
		interact = GetNode<Sprite>("Interact");
		npcColorStr = $"#{npcColor.ToArgb32().ToString("X").Substring(2)}";

		spr.Frames = npcSprite;
		interact.Hide();
    }


	public override void _Process(float delta)
	{
		ZIndex = (int)Position.y;

		if (Input.IsActionJustPressed("sys_accept") && Player.State == Player.ST.MOVE && interact.Visible)
		{
			Player.State = Player.ST.NO_INPUT;
			interact.Hide();
			Controller.Dialogue(dialogueFile, dialogueSet, "Ravia", "#2391ef",  raviaPortrait, npcName, npcColorStr, npcPortrait, signalConnection: this, signalMethod: "EndDialogue");
		}
	}

	// ================================================================

	private void EndDialogue()
	{
		Player.State = Player.ST.MOVE;
		interact.Show();
		dialogueSet = Mathf.Min(dialogueSet + 1, maxDialogueSet);
	}


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
