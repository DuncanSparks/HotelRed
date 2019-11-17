using Godot;
using System;

public class L2_Center : Node2D
{
	[Export]
	private NodePath child;

	public override void _Ready()
	{
		if (Controller.Flag("scene_child_1") == 1)
		{
			GetNode<Sprite>(child).QueueFree();
		}
	}
}
