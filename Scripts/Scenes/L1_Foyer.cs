using Godot;
using System;

public class L1_Foyer : Node2D
{
	public override void _Ready()
	{
		if (Controller.Flag("enter_foyer") == 1)
			GetNode<Node2D>("SoulFade2").GetNode<ColorRect>("ColorRect").QueueFree();
	}
}