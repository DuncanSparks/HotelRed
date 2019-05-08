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
	private SpriteDirection startDirection = SpriteDirection.DOWN;

	[Export]
	private int maxDialogueSet;

	[Export]
	private bool autoAdvanceSet;

	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile = string.Empty;

	[Export]
	private bool itemGiver = false;
	
	[Export]
	private int itemIndex = -1;


	
	// Motion
	private Vector2 motion = new Vector2(0, 0);
	private SpriteDirection face = SpriteDirection.DOWN;
	private bool walking = false;
	private float walkSpeed = 0f;

	public enum SpriteDirection {UP, DOWN, LEFT, RIGHT};

	private int dialogueSet = 0;
	private string npcColorStr;
	private SpriteFrames raviaPortrait = GD.Load<SpriteFrames>("res://Resources/Portrait Sets/Portraits_Ravia.tres");

	// Refs
	private AnimatedSprite spr;
	private Sprite interact;

	// ================================================================

	public Vector2 Motion { get; set; }
	public SpriteDirection Face { get; set; }
	public bool Walking { get; set; }
	public float WalkSpeed { get; set; }

	// ================================================================

    public override void _Ready()
    {
		spr = GetNode<AnimatedSprite>("Sprite");
		interact = GetNode<Sprite>("Interact");
		npcColorStr = $"#{npcColor.ToArgb32().ToString("X").Substring(2)}";

		spr.Frames = npcSprite;
		face = startDirection;
		interact.Hide();
    }


	public override void _Process(float delta)
	{
		ZIndex = (int)Position.y;

		Animate();

		if (Input.IsActionJustPressed("sys_accept") && Player.State == Player.ST.MOVE && interact.Visible)
		{
			Player.State = Player.ST.NO_INPUT;
			interact.Hide();
			Controller.Dialogue(dialogueFile, dialogueSet, "Ravia", "#2391ef",  raviaPortrait, npcName, npcColorStr, npcPortrait, signalConnection: this, signalMethod: "EndDialogue");
			if(itemGiver)
			{
				Player.Main.AddItem(itemIndex);
			}
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
		if (area.IsInGroup("PlayerSight") && Player.State == Player.ST.MOVE)
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


	private void Animate()
	{
		switch (face)
		{
			case SpriteDirection.UP:
				spr.Play(walking ? "walkup" : "up");
				break;
			case SpriteDirection.DOWN:
				spr.Play(walking ? "walkdown" : "down");
				break;
			case SpriteDirection.LEFT:
				spr.Play(walking ? "walkleft" : "left");
				break;
			case SpriteDirection.RIGHT:
				spr.Play(walking ? "walkright" : "right");
				break;
		}
	}
}
