using Godot;
using System;

public class EventNPC : KinematicBody2D
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
	private bool destroyAfterEvent = false;

	[Export]
	private bool wantsItem = false;

	[Export]
	private Player.Items requiredItem;

	[Export]
	private int requiredItemEvent;

	[Export]
	private string indexFlag = string.Empty;

	// Motion
	private Vector2 motion = new Vector2(0, 0);
	private SpriteDirection face = SpriteDirection.DOWN;
	private bool walking = false;
	private float walkSpeed = 0f;
	private bool depthControl = true;

	public enum SpriteDirection {UP, DOWN, LEFT, RIGHT};
	private bool spriteOverride;

	private int dialogueSet = 0;
	private string npcColorStr;
	private SpriteFrames raviaPortrait = GD.Load<SpriteFrames>("res://Resources/Portrait Sets/Portraits_Ravia.tres");

	// Refs
	private AnimatedSprite spr;
	private Sprite interact;
	private Event ev;

	// ================================================================

	public Vector2 Motion { get { return motion; } set { motion = value; } }
	public SpriteDirection Face { get { return face; } set { face = value; } }
	public bool Walking { get { return walking; } set { walking = value; } }
	public float WalkSpeed { get { return walkSpeed; } set { walkSpeed = value; } }
	public bool DepthControl { get => depthControl; set => depthControl = value; }
	public bool SpriteOverride { get => spriteOverride; set => spriteOverride = value; }

	// ================================================================

    public override void _Ready()
    {
		spr = GetNode<AnimatedSprite>("Sprite");
		interact = GetNode<Sprite>("Interact");
		ev = GetNode<Event>("Event");
		npcColorStr = $"#{npcColor.ToArgb32().ToString("X").Substring(2)}";
		
		spr.Frames = npcSprite;
		face = startDirection;
		interact.Hide();

		if (indexFlag != string.Empty)
			dialogueSet = Controller.Flag(indexFlag);
    }


	public override void _Process(float delta)
	{
		if (depthControl)
			ZIndex = (int)Position.y;

		if (!spriteOverride)
			Animate();

		if (Input.IsActionJustPressed("sys_accept") && Player.State == Player.ST.MOVE && interact.Visible)
		{
			Player.State = Player.ST.NO_INPUT;
			interact.Hide();
			
			if (wantsItem && Player.HasItem(requiredItem))
			{
				Player.RemoveItem(requiredItem);
				ev.StartEvent(requiredItemEvent - 1);
			}
			else
				ev.StartEvent(dialogueSet);
		}
	}


	public override void _PhysicsProcess(float delta)
	{
		MoveAndSlide(motion * walkSpeed);
	}

	// ================================================================

	public void PlayAnimation(string animation)
	{
		spr.Play(animation);
	}


	public void EndDialogue()
	{
		Player.State = Player.ST.MOVE;
		interact.Show();
		dialogueSet = Mathf.Min(++dialogueSet, maxDialogueSet);

		if (indexFlag != string.Empty)
			Controller.SetFlag(indexFlag, dialogueSet);
	}

	// ================================================================

	private void InteractAreaEntered(Area2D area)
	{
		if (area.IsInGroup("PlayerSight") && Player.State == Player.ST.MOVE)
			interact.Show();
	}


	private void InteractAreaExited(Area2D area)
	{
		if (area.IsInGroup("PlayerSight"))
			interact.Hide();
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


	private void EndEventParent(string anim_name)
	{
		if (destroyAfterEvent)
			QueueFree();
	}
}
