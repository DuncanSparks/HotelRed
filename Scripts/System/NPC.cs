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
	private Player.Items itemIndex;

	[Export]
	private string indexFlag = string.Empty;

	
	// Motion
	private Vector2 motion = new Vector2(0, 0);
	private SpriteDirection face = SpriteDirection.DOWN;
	private bool walking = false;
	private float walkSpeed = 0f;

	public enum SpriteDirection {UP, DOWN, LEFT, RIGHT};

	private int dialogueSet = 0;
	private string npcColorStr;
	private SpriteFrames raviaPortrait = GD.Load<SpriteFrames>("res://Resources/Portrait Sets/Portraits_Ravia.tres");

	private bool disabled = false;

	// Refs
	private AnimatedSprite spr;
	private Sprite interact;

	// ================================================================

	public Vector2 Motion { get => motion; set => motion = value; }
	public SpriteDirection Face { get => face; set => face = value; }
	public bool Walking { get => walking; set => walking = value; }
	public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
	public bool Disabled { get => disabled; set => disabled = value; }
	
	// ================================================================

	public override void _Ready()
	{
		spr = GetNode<AnimatedSprite>("Sprite");
		interact = GetNode<Sprite>("Interact");
		npcColorStr = $"#{npcColor.ToArgb32().ToString("X").Substring(2)}";

		spr.Frames = npcSprite;
		face = startDirection;
		interact.Hide();

		if (indexFlag != string.Empty)
			dialogueSet = Controller.Flag(indexFlag);
	}


	public override void _Process(float delta)
	{
		spr.Visible = !disabled;

		if (!disabled)
		{
			ZIndex = (int)Position.y;
			Animate();

			if (Input.IsActionJustPressed("sys_accept") && Player.State == Player.ST.MOVE && interact.Visible)
			{
				Player.State = Player.ST.NO_INPUT;
				interact.Hide();
				ChangeDirection();
				Controller.Dialogue(dialogueFile, dialogueSet, "Ravia", "#2391ef",  raviaPortrait, npcName, npcColorStr, npcPortrait, signalConnection: this, signalMethod: "EndDialogue");
				if (itemGiver && dialogueSet == 0)
					Player.AddItem(itemIndex);
			}
		}
	}

	// ================================================================

	private void EndDialogue()
	{
		Player.State = Player.ST.MOVE;
		interact.Show();
		dialogueSet = Mathf.Min(++dialogueSet, maxDialogueSet);

		if (indexFlag != string.Empty)
			Controller.SetFlag(indexFlag, dialogueSet);
	}


	private void InteractAreaEntered(Area2D area)
	{
		if (!disabled && area.IsInGroup("PlayerSight") && Player.State == Player.ST.MOVE)
		{
			interact.Show();
		}
	}


	private void InteractAreaExited(Area2D area)
	{
		if (!disabled && area.IsInGroup("PlayerSight"))
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


	private void ChangeDirection()
	{
		switch (Player.Face)
		{
			case Player.SpriteDirection.UP:
				face = SpriteDirection.DOWN;
				break;
			case Player.SpriteDirection.DOWN:
				face = SpriteDirection.UP;
				break;
			case Player.SpriteDirection.LEFT:
				face = SpriteDirection.RIGHT;
				break;
			case Player.SpriteDirection.RIGHT:
				face = SpriteDirection.LEFT;
				break;
		}
	}
}
