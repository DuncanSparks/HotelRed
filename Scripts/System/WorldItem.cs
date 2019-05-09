using Godot;
using System;

public class WorldItem : StaticBody2D
{
	[Export]
	private string itemName = string.Empty;

	[Export]
	private Player.Items itemId;

	[Export]
	private string removeFlag = string.Empty;

	[Export]
	private AudioStream getSound;

	private bool inSight = false;

	// Refs
	private Sprite spr;

	// ================================================================

	public override void _Ready()
	{
		if (Controller.Flag(removeFlag) == 1)
			QueueFree();

		spr = GetNode<Sprite>("Sprite");
	}


	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("sys_accept") && Player.State == Player.ST.MOVE && inSight)
			Collect();
	}

	// ================================================================

	private void Collect()
	{
		Controller.PlaySoundBurst(getSound);
		Player.AddItem(itemId);
		Controller.SetFlag(removeFlag, 1);
		spr.Hide();
		Player.ShowInteract(false);
		SetVisible(false);
	}


	private void SightEntered(Area2D area)
	{
		if (area.IsInGroup("PlayerSight"))
		{
			inSight = true;
			Player.ShowInteract(true);
		}
			
	}


	private void SightExited(Area2D area)
	{
		if (area.IsInGroup("PlayerSight"))
		{
			inSight = false;
			Player.ShowInteract(false);
		}
	}
}
