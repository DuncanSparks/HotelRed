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
	private AudioStream getSound;

	private bool active = false;
	private bool inSight = false;

	private PackedScene itemNameRef = GD.Load<PackedScene>("res://Instances/ItemName.tscn");

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
		Controller.PlaySoundBurst(getSound);
		Player.AddItem(itemId);
		Controller.SetFlag(removeFlag, 1);
		spr.Hide();
		Player.ShowInteract(false);
		var name = (ItemName)itemNameRef.Instance();
		name.SetItemName(itemName);
		name.SetPosition(Position);
		GetTree().GetRoot().AddChild(name);
		QueueFree();
		//SetVisible(false);
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
}
