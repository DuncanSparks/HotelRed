using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
	private static Player inst;
    public static Player Main { get { return inst; } }

	Player()
	{
		inst = this;
	}

	// ================================================================

	[Export]
	private SpriteFrames debugSpriteFrames;

	[Export]
	private SpriteFrames debugSpriteFrames2;

	[Export(PropertyHint.File, "*.txt")]
	private string debugDialogueFile = string.Empty;

	[Export(PropertyHint.File, "*.txt")]
	private string debugDialogueFile2 = string.Empty;

	// ================================================================

	// Movement
	private Vector2 motion = new Vector2(0, 0);
	private SpriteDirection face = SpriteDirection.DOWN;
	private bool walking = false;
	private bool teleporting = false;

	private bool motionOverride = false;
	private Vector2 motionOverrideVec = new Vector2(0, 0);
	private float walkSpeedOverride = 180f;

	// Sprite sets 
	public enum SpriteSet {NORMAL, PAPER};
	public enum SpriteDirection {UP, DOWN, LEFT, RIGHT};
	private SpriteSet currentSpriteSet = SpriteSet.NORMAL;

	private static readonly string[] spriteSetNormal = {"up", "down", "left", "right"};
	private static readonly string[] spriteSetNormalWalk = {"walkup", "walkdown", "walkleft", "walkright"};
	private static readonly string[] spriteSetPaper = {"up", "down_paper", "left", "right_paper"};
	private static readonly string[] spriteSetPaperWalk = {"walkup", "walkdown_paper", "walkleft", "walkright_paper"};

	// Step sounds
	private static readonly string[] stepSounds = {"SoundStep1", "SoundStep2", "SoundStep3", "SoundStep4", "SoundStep5"};
	private float sound = -1;

	// States
	public enum ST {MOVE, NO_INPUT};
	private ST state = ST.MOVE;

	// Constants
	private const float WalkSpeed = 180f;
	private const float StepSoundInterval = (1f / 7f) * 2f;

	// Refs
	private AnimatedSprite Spr;

	private Timer TimerStepSound;

	// Inventory refs
	private Label CurrentItemName;

    private HBoxContainer Images;

    private Label CurrentItemDescription;

	private Control ItemList;

	private bool inventoryLock = true;

	private int numItems = 0;




	// ================================================================

	public static ST State { get { return Player.Main.state; } set { Player.Main.state = value; } }
	public static Vector2 Motion { get { return Player.Main.motion; } set { Player.Main.motion = value; } }
	public static bool MotionOverride { get { return Player.Main.motionOverride; } set { Player.Main.motionOverride = value; } }
	public static Vector2 MotionOverrideVec { get { return Player.Main.motionOverrideVec; } set { Player.Main.motionOverrideVec = value; } }
	public static float WalkSpeedOverride { get { return Player.Main.walkSpeedOverride; } set {Player.Main.walkSpeedOverride = value; } }
	public static SpriteDirection Face { get { return Player.Main.face; } set { Player.Main.face = value; } }
	public static bool Walking { get { return Player.Main.walking; } set { Player.Main.walking = value; } }
	public static bool Teleporting { get { return Player.Main.teleporting; } set { Player.Main.teleporting = value; } }
	public static SpriteSet CurrentSpriteSet { get { return Player.Main.currentSpriteSet; } set { Player.Main.currentSpriteSet = value; } }
	public static int NumItems { get { return Player.Main.numItems; } set { Player.Main.numItems = value; } }

	public static bool InventoryLock { get { return Player.Main.inventoryLock; } set { Player.Main.inventoryLock = value; } }
	// ================================================================

	// Inventory

	// private PackedScene RoomKeyRef = GD.Load<PackedScene>("res://Instances/Items/RoomKey.tscn");
	// ================================================================
	public override void _Ready()
	{
		// Refs
		
		Spr = GetNode<AnimatedSprite>("Sprite");
		TimerStepSound = GetNode<Timer>("TimerStepSound");

		ItemList = GetNode<CanvasLayer>("CanvasLayer").GetNode<Control>("Inventory");
		CurrentItemName = ItemList.GetNode<Label>("CurrentItem");
        Images = ItemList.GetNode<HBoxContainer>("Container");
        CurrentItemDescription = ItemList.GetNode<Label>("CurrentDescription");

		var cont = ItemList.GetNode<HBoxContainer>("Container");
		CurrentItemName.Text = cont.GetNode<Item>("RoomKey").ItemName;
		CurrentItemDescription.Text = cont.GetNode<Item>("RoomKey").ItemDescription;
		// numItems += 1;
		// Images.GetNode<TextureRect>("Item1")
		// GetTree().GetRoot().AddChild(RoomKey);
		// Images.GetNode<TextureRect>("Item1").SetTexture(RoomKey.Image.Texture);
	}

	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("open_inventory") && !inventoryLock)
		{
			if(ItemList.IsVisible())
				ItemList.SetVisible(false);
			else
				ItemList.SetVisible(true);
		}
		if (walking)
			sound += 1 * delta;
		else
			sound = -1;

		if (walking && TimerStepSound.IsStopped())
		{
			FootstepSound();
			TimerStepSound.SetWaitTime(StepSoundInterval);
			TimerStepSound.Start();
		}
		else if (!walking)
			TimerStepSound.Stop();
	}

	public override void _PhysicsProcess(float delta)
	{
		ZIndex = (int)Position.y;

		switch (state)
		{
			case ST.MOVE:
			{
				Movement();
				walking = motion.x != 0 || motion.y != 0;
				break;
			}

			case ST.NO_INPUT:
			{
				motion = Vector2.Zero;
				break;
			}
		}

		Spr.Play(GetSprite(currentSpriteSet, face, walking));

		if (!motionOverride)
			motion = MoveAndSlide(motion * WalkSpeed * delta * 60f);
		else
			motion = MoveAndSlide(motionOverrideVec * walkSpeedOverride * delta * 60f);
	}

	// ================================================================

	public static Camera2D GetCamera()
	{
		return Player.Main.GetNode<Camera2D>("Camera");
	}


	public static void EnableCamera(bool enable)
	{
		Player.Main.GetNode<Camera2D>("Camera").Current = enable;
	}

	// ================================================================

	private void Movement()
	{
		// Movement
		int lMove = Input.IsActionPressed("move_left") ? 1 : 0;
		int rMove = Input.IsActionPressed("move_right") ? 1 : 0;
		motion.x = rMove - lMove;

		int uMove = Input.IsActionPressed("move_up") ? 1 : 0;
		int dMove = Input.IsActionPressed("move_down") ? 1 : 0;
		motion.y = dMove - uMove;

		// Change face direction
		if (motion.x == 0)
		{
			switch (motion.y)
			{
				case -1:
					face = SpriteDirection.UP;
					break;
				case 1:
					face = SpriteDirection.DOWN;
					break;
			}
		}
		else if (motion.y == 0)
		{
			switch (motion.x)
			{
				case -1:
					face = SpriteDirection.LEFT;
					break;
				case 1:
					face = SpriteDirection.RIGHT;
					break;
			}
		}
	}


	private void FootstepSound()
	{
		Controller.PlaySoundBurst(GetNode<AudioStreamPlayer>(Tools.Choose<string>(stepSounds)).Stream, volume: -4f, pitch: (float)GD.RandRange(0.9, 1.1));
	}


	private string GetSprite(SpriteSet set, SpriteDirection direction, bool walking)
	{
		switch (set)
		{
			case SpriteSet.NORMAL:
				return walking ? spriteSetNormalWalk[(int)direction] : spriteSetNormal[(int)direction];

			case SpriteSet.PAPER:
				return walking ? spriteSetPaperWalk[(int)direction] : spriteSetPaper[(int)direction];
			
			default:
				return walking ? spriteSetNormalWalk[(int)direction] : spriteSetNormal[(int)direction];
		}
	}

    private class RoomKey
    {
    }
}
