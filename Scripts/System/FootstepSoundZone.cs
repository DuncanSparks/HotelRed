using Godot;
using System;

public class FootstepSoundZone : Area2D
{
    [Export]
	private Player.Sound sound = Player.Sound.CONCRETE;

	// ================================================================

	void PlayerEntered(PhysicsBody2D body)
	{
		if (body.IsInGroup("Player"))
			Player.StepSound = sound;
	}
}
