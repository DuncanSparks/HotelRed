using Godot;
using System;

public class Event : Area2D
{
	[Export]
	private bool destroy = true;

	[Export]
	private string destroyFlag = string.Empty;

	[Export]
	private bool autoStart = false;

	private bool running = false;

	// Refs
	private AnimationPlayer AnimPlayer;

	// ================================================================

    public override void _Ready()
    {
        if (destroy && Controller.Flag(destroyFlag))
			QueueFree();

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		SetProcess(autoStart);
    }


	public override void _Process(float delta)
	{
		if (Player.State == Player.ST.MOVE)
		{
			StartEvent();
			running = true;
			SetProcess(false);
		}
	}

	// ================================================================

	public void PauseEvent()
	{
		AnimPlayer.Stop(false);
	}

	
	public void ResumeEvent()
	{
		AnimPlayer.Play();
	}


	public void EndEvent(string anim_name)
	{
		if (anim_name == "Event")
		{
			Player.State = Player.ST.MOVE;
			Player.MotionOverride = false;
			QueueFree();
		}
	}


	public void StopPlayer(Player.SpriteDirection direction)
	{
		Player.Walking = false;
		Player.State = Player.ST.NO_INPUT;
		Player.Motion = new Vector2(0, 0);
		Player.MotionOverrideVec = new Vector2(0, 0);
		Player.Face = direction;
	}

	// ================================================================

	private void StartEvent()
	{
		StopPlayer(Player.Face);
		AnimPlayer.Play("Event");
	}

	private void BodyEntered(PhysicsBody2D body)
	{
		if (!autoStart && !running && body.IsInGroup("Player") && Player.State == Player.ST.MOVE)
		{
			StartEvent();
			running = true;
		}
	}
}
