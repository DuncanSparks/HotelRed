using Godot;
using System;

public class Event : Area2D
{
	[Export]
	private bool destroyOnEnd = true;

	[Export]
	private bool destroy = true;

	[Export]
	private bool npc = false;

	[Export]
	private string destroyFlag = string.Empty;

	[Export]
	private bool autoStart = false;

	private bool running = false;
	private bool eventStarted = false;

	// Refs
	private AnimationPlayer AnimPlayer;

	// ================================================================

	public override void _Ready()
	{
		if (destroy)
		{
			if (Controller.Flag(destroyFlag) == 1)
				QueueFree();
			//else
				//Controller.SetFlag(destroyFlag, 1);
		}

		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		SetProcess(autoStart);
	}


	public override void _Process(float delta)
	{
		if (Player.State == Player.ST.MOVE)
		{
			StartEvent(0);
			running = true;
			SetProcess(false);
		}
	}


	public override void _ExitTree()
	{
		if (destroy && eventStarted)
			Controller.SetFlag(destroyFlag, 1);
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
		if (anim_name.Substr(0, 5) == "Event")
		{
			Player.FreePlayer();

			if (npc)
				GetParent<EventNPC>().EndDialogue();
			
			if (destroyOnEnd)
				QueueFree();
		}
		
		Player.InventoryLock = false;
	}


	public void StopPlayer(Player.SpriteDirection direction)
	{
		Player.StopPlayer();
	}

	// ================================================================

	public void StartEvent(int index)
	{
		StopPlayer(Player.Face);
		AnimPlayer.Play("Event" + (index > 0 ? (index + 1).ToString() : ""));
		Player.InventoryLock = true;
	}


	private void BodyEntered(PhysicsBody2D body)
	{
		if (!autoStart && !running && body.IsInGroup("Player") && Player.State == Player.ST.MOVE)
		{
			StartEvent(0);
			running = true;
			eventStarted = true;
		}
	}
}
