using Godot;
using System;

public class RecRoom : Node2D
{
	[Export]
	private NodePath npc;

    public override void _Ready()
    {
        if (Controller.Flag("item_paper") == 1)
			GetNode<EventNPC>(npc).QueueFree();
    }
}
