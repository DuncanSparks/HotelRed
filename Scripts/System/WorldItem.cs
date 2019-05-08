using Godot;
using System;

public class WorldItem : StaticBody2D
{
    [Export]
	private string itemName = string.Empty;

	[Export]
	private string removeFlag = string.Empty;

	// ================================================================

	public override void _Ready()
	{
		if (Controller.Flag(removeFlag) == 1)
			QueueFree();
	}

	// ================================================================

	private void SightEntered(Area2D area)
	{
		if (area.IsInGroup("PlayerSight"))
			{}
	}


	private void SightExited(Area2D area)
	{
		if (area.IsInGroup("PlayerSight"))
			{}
	}
}
