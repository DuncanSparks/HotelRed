using Godot;
using System;

public class WorldItem : StaticBody2D
{
	[Export]
	private bool startActive = true;

	[Export]
	private string itemName = string.Empty;

	[Export]
	private Player.Items itemId;

	[Export]
	private string removeFlag = string.Empty;

	[Export]
	private AudioStream getSound = null;

	private bool active = false;
	private bool inSight = false;

	// Refs
	private Sprite spr;

	// ================================================================

	public override void _Ready()
	{
		if (Controller.Flag(removeFlag) == 1)
			QueueFree();

		active = startActive;

		spr = GetNode<Sprite>("Sprite");
	}


	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("sys_accept") && Player.State == Player.ST.MOVE && inSight && active)
			Collect();
	}

	// ================================================================

	public void Activate()
	{
		active = true;
	}

	// ================================================================

	private void Collect()
	{
		if (getSound != null)
			Controller.PlaySoundBurst(getSound);

		Player.AddItem(itemId, itemName);
		Controller.SetFlag(removeFlag, 1);
		spr.Hide();
		Player.ShowInteract(false);
		QueueFree();
	}


	private void SightEntered(Area2D area)
	{
		if (area.IsInGroup("PlayerSight") && active)
		{
			inSight = true;
			Player.ShowInteract(true);
		}
	}


	private void SightExited(Area2D area)
	{
		if (area.IsInGroup("PlayerSight") && active)
		{
			inSight = false;
			Player.ShowInteract(false);
		}
	}


	private void SightEnteredOverride()
	{
		inSight = true;
		Player.ShowInteract(true);
	}
}
