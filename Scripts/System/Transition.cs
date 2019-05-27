using Godot;
using System;

public class Transition : Area2D
{
	[Export(PropertyHint.File, "*.tscn")]
	private string targetScene = string.Empty;

	[Export]
	private Vector2 targetPosition;

    [Export]
	private Direction targetDirection;

	[Export]
	private bool walk;

	[Export]
	private bool isLocked = false;

	[Export]
	private int itemIndex = -1;

	[Export]
	private bool playsSoundOnEnter = false;

	[Export]
	private AudioStream getSound;
	// Constants
	private const int WalkOffset = 64;

	// Refs
	private Timer TimerFadeOut;
	private Timer TimerTransition;
	private Timer TimerFadeIn;

	// ================================================================

	private enum Direction {Up, Down, Left, Right};

	// ================================================================

    public override void _Ready()
    {
		if (isLocked)
		{
			GetChild<CollisionShape2D>(0).SetDisabled(true);
		}
		if (isLocked && itemIndex != -1 && Player.itemsCollected[itemIndex] == true)
		{
		 	GetChild<CollisionShape2D>(0).SetDisabled(false);
		}
        TimerFadeOut = GetNode<Timer>("TimerFadeOut");
		TimerTransition = GetNode<Timer>("TimerTransition");
		TimerFadeIn = GetNode<Timer>("TimerFadeIn");
    }

	// ================================================================

	private void BodyEntered(PhysicsBody2D body)
	{
		if (!Player.Teleporting && body.IsInGroup("Player"))
		{
			Player.Teleporting = true;
			Player.State = Player.ST.NO_INPUT;

			if (walk)
			{
				Player.Walking = true;
				Player.MotionOverride = true;
				Player.WalkSpeedOverride = 130f;

				switch (targetDirection)
				{
					case Direction.Up:
						Player.Face = Player.SpriteDirection.UP;
						Player.MotionOverrideVec = new Vector2(0, -1);
						break;
					case Direction.Down:
						Player.Face = Player.SpriteDirection.DOWN;
						Player.MotionOverrideVec = new Vector2(0, 1);
						break;
					case Direction.Left:
						Player.Face = Player.SpriteDirection.LEFT;
						Player.MotionOverrideVec = new Vector2(-1, 0);
						break;
					case Direction.Right:
						Player.Face = Player.SpriteDirection.RIGHT;
						Player.MotionOverrideVec = new Vector2(1, 0);
						break;
					default:
						Player.Face = Player.SpriteDirection.UP;
						Player.MotionOverrideVec = new Vector2(0, -1);
						break;
				}
				TimerFadeOut.Start();
			}
			else
			{
				Player.Walking = false;
				StartFadeOut();
			}
		}
	}


	private void StartFadeOut()
	{
		if (playsSoundOnEnter)
			Controller.PlaySoundBurst(getSound);
		Controller.Fade(false, false, 0.25f);

		TimerTransition.Start();
	}


	private void ChangeScenes()
	{
		Controller.SceneGoto(targetScene);
		
		if (!walk)
			Player.Main.Position = targetPosition;
		else
		{
			switch (targetDirection)
			{
				case Direction.Up:
					Player.Main.Position = new Vector2(targetPosition.x, targetPosition.y + WalkOffset);
					break;
				case Direction.Down:
					Player.Main.Position = new Vector2(targetPosition.x, targetPosition.y - WalkOffset);
					break;
				case Direction.Left:
					Player.Main.Position = new Vector2(targetPosition.x + WalkOffset, targetPosition.y);
					break;
				case Direction.Right:
					Player.Main.Position = new Vector2(targetPosition.x - WalkOffset, targetPosition.y);
					break;
				default:
					Player.Main.Position = new Vector2(targetPosition.x, targetPosition.y + WalkOffset);
					break;
			}
		}

		Controller.EndTransition();
	}


	private void StartFadeIn()
	{
		Controller.Fade(true, false, 0.25f);
	}
}
