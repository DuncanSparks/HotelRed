//#define DEBUG_FAST_WALK
#define DEBUG_ENABLE_MOVE

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
	public enum SpriteSet {NORMAL, KNEEL};
	public enum SpriteDirection {UP, DOWN, LEFT, RIGHT};
	public enum Items {Room_Key, Bat_Hearts, Sleeper_Key, Shredded_Paper, Water, Coin, uh, Tape1, Tape2, Tape3};
	private SpriteSet currentSpriteSet = SpriteSet.NORMAL;
	private bool spriteOverride = false;

	private static readonly string[] spriteSetNormal = {"up", "down", "left", "right"};
	private static readonly string[] spriteSetNormalWalk = {"walkup", "walkdown", "walkleft", "walkright"};
	private static readonly string[] spriteSetPaper = {"up", "down_paper", "left", "right_paper"};
	//private static readonly string[] spriteSetPaperWalk = {"walkup", "walkdown_paper", "walkleft", "walkright_paper"};
	private static readonly string[] spriteSetKneel = {"up_kneel", "down_kneel", "left_kneel", "right_kneel"};

	// Step sounds
	private static readonly string[] stepSounds = {"SoundStep1", "SoundStep2", "SoundStep3", "SoundStep4", "SoundStep5"};
	private static readonly string[] stepSoundsConc = {"SoundStepConc1", "SoundStepConc2", "SoundStepConc3", "SoundStepConc4"};
	private static readonly string[] stepSoundsCarp = {"SoundStepCarp1", "SoundStepCarp2", "SoundStepCarp3", "SoundStepCarp4", "SoundStepCarp5"};
	public enum Sound {WOOD, CONCRETE, CARPET};
	private Sound stepSound = Sound.CONCRETE;

	//Invetory Sound
	[Export]
	private string inventorySound = "InventorySound";
	
	private float sound = -1;

	// States
	public enum ST {MOVE, NO_INPUT};

	#if DEBUG_ENABLE_MOVE
		private ST state = ST.MOVE;
	#else
		private ST state = ST.NO_INPUT;
	#endif

	// Constants
	#if DEBUG_FAST_WALK
		private const float WalkSpeed = 300f;
	#else
		private const float WalkSpeed = 180f;
	#endif
	
	private const float StepSoundInterval = (1f / 7f) * 2f;

	// Refs
	private AnimatedSprite Spr;
	private Timer TimerStepSound;
	private Area2D Sight;

	// Inventory refs
	private Label CurrentItemName;
    private HBoxContainer images;
    private Label CurrentItemDescription;
	private Control inventory;
	private bool inventoryLock = false;
	private bool canViewInventory = true;
	private bool depthControl = true;

	private int numItems = 0;
	
	private Control codeOverlay;
	// private bool canViewInventory = true;

	// ================================================================

	public static ST State { get { return Player.Main.state; } set { Player.Main.state = value; } }
	public static Vector2 Motion { get { return Player.Main.motion; } set { Player.Main.motion = value; } }
	public static bool MotionOverride { get { return Player.Main.motionOverride; } set { Player.Main.motionOverride = value; } }
	public static Vector2 MotionOverrideVec { get { return Player.Main.motionOverrideVec; } set { Player.Main.motionOverrideVec = value; } }
	public static float WalkSpeedOverride { get { return Player.Main.walkSpeedOverride; } set {Player.Main.walkSpeedOverride = value; } }
	public static SpriteDirection Face { get { return Player.Main.face; } set { Player.Main.face = value; } }
	public static bool SpriteOverride { get => Player.Main.spriteOverride; set => Player.Main.spriteOverride = value; }
	public static bool Walking { get { return Player.Main.walking; } set { Player.Main.walking = value; } }
	public static bool Teleporting { get { return Player.Main.teleporting; } set { Player.Main.teleporting = value; } }
	public static Sound StepSound{ get { return Player.Main.stepSound; } set { Player.Main.stepSound = value; } }
	public static SpriteSet CurrentSpriteSet { get { return Player.Main.currentSpriteSet; } set { Player.Main.currentSpriteSet = value; } }
	public static int NumItems { get { return Player.Main.numItems; } set { Player.Main.numItems = value; } }
	public static bool InventoryLock { get { return Player.Main.inventoryLock; } set { Player.Main.inventoryLock = value; } }
	// public static bool CanViewInventory { get { return Player.Main.canViewInventory; } set { Player.Main.canViewInventory = value; } }
	public static Control Inventory { get { return Player.Main.inventory; } set { Player.Main.inventory = value; } }
	public static Control CodeOverlay { get { return Player.Main.codeOverlay; } set { Player.Main.codeOverlay = value; } }
	public static HBoxContainer Images { get { return Player.Main.images; } set { Player.Main.images = value; } }
	public static bool DepthControl { get => Player.Main.depthControl; set => Player.Main.depthControl = value; }
	
	public static bool[] itemsCollected = {false, false, false, false, false, false, false, false, false, false};
	// Utility properties
	public static Vector2 BubblePosition { get { return new Vector2(Player.Main.Position.x + 6, Player.Main.Position.y - 56); } }

	// ================================================================
	
	public override void _Ready()
	{
		// Refs	
		
		Spr = GetNode<AnimatedSprite>("Sprite");
		TimerStepSound = GetNode<Timer>("TimerStepSound");
		Sight = GetNode<Area2D>("Sight");
		inventory = GetNode<CanvasLayer>("CanvasLayer").GetNode<Control>("Inventory");
		codeOverlay = GetNode<CanvasLayer>("CanvasLayer").GetNode<Control>("CodeOverlay");
		CurrentItemName = inventory.GetNode<Label>("CurrentItem");
        Images = inventory.GetNode<HBoxContainer>("Container");
        CurrentItemDescription = inventory.GetNode<Label>("CurrentDescription");
		//AddItem(0);

		#if DEBUG_ENABLE_MOVE
			EnableCamera(true);
		#endif
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("open_inventory") && !inventoryLock && canViewInventory)
		{
			Controller.PlaySoundBurst(GetNode<AudioStreamPlayer>(inventorySound).Stream);
			if (inventory.IsVisible())
			{
				FreePlayer();
				inventory.SetVisible(false);
			}
			else
			{
				StopPlayer();
				inventory.SetVisible(true);
			}
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

		//if (Input.IsActionJustPressed("debug_1"))
			//Controller.SaveGame();
	}

	public override void _PhysicsProcess(float delta)
	{
		if (depthControl)
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

		if (!spriteOverride)
			Spr.Play(GetSprite(currentSpriteSet, face, walking));

		if (!motionOverride)
			motion = MoveAndSlide(motion * WalkSpeed);
		else
			motion = MoveAndSlide(motionOverrideVec * walkSpeedOverride);
	}

	// ================================================================
	
	public static void StopPlayer()
	{
		Player.Walking = false;
		Player.State = Player.ST.NO_INPUT;
		Player.Motion = new Vector2(0, 0);
		Player.MotionOverrideVec = new Vector2(0, 0);
	}


	public static void FreePlayer()
	{
		Player.State = Player.ST.MOVE;
		Player.MotionOverride = false;
	}


	public static void PlayDoorAnimation()
	{
		switch (Face)
		{
			case SpriteDirection.UP:
				Player.Main.Spr.Play("up_door");
				break;
		}
	}


	public static void ShowInteract(bool show)
	{
		Player.Main.GetNode<Sprite>("Interact").Visible = show;
	}


	public static Camera2D GetCamera()
	{
		return Player.Main.GetNode<Camera2D>("Camera");
	}


	public static void EnableCamera(bool enable)
	{
		Player.Main.GetNode<Camera2D>("Camera").Current = enable;
	}


	public static void AddItem(Items item)
	{
		Player.Images.GetChild<TextureButton>((int)item).SetVisible(true);
		itemsCollected[(int)item] = true;
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

		// Change sight face
		switch (face)
		{
			case SpriteDirection.UP:
				Sight.RotationDegrees = 180;
				break;
			case SpriteDirection.DOWN:
				Sight.RotationDegrees = 0;
				break;
			case SpriteDirection.LEFT:
				Sight.RotationDegrees = 90;
				break;
			case SpriteDirection.RIGHT:
				Sight.RotationDegrees = 270;
				break;
		}
	}


	private void FootstepSound()
	{
		switch (stepSound)
		{
			case Sound.WOOD:
				Controller.PlaySoundBurst(GetNode<AudioStreamPlayer>(Tools.Choose<string>(stepSounds)).Stream, volume: -4f, pitch: (float)GD.RandRange(0.9, 1.1));
				break;
			case Sound.CONCRETE:
				Controller.PlaySoundBurst(GetNode<AudioStreamPlayer>(Tools.Choose<string>(stepSoundsConc)).Stream, volume: 1f, pitch: (float)GD.RandRange(0.9, 1.1));
				break;
			case Sound.CARPET:
				Controller.PlaySoundBurst(GetNode<AudioStreamPlayer>(Tools.Choose<string>(stepSoundsCarp)).Stream, volume: 6f, pitch: (float)GD.RandRange(0.9, 1.1));
				break;
		}
	}


	private string GetSprite(SpriteSet set, SpriteDirection direction, bool walking)
	{
		switch (set)
		{
			case SpriteSet.NORMAL:
				return walking ? spriteSetNormalWalk[(int)direction] : spriteSetNormal[(int)direction];

			case SpriteSet.KNEEL:
				//return walking ? spriteSetPaperWalk[(int)direction] : spriteSetPaper[(int)direction];
				return spriteSetKneel[(int)direction];
			
			default:
				return walking ? spriteSetNormalWalk[(int)direction] : spriteSetNormal[(int)direction];
		}
	}

	
}
