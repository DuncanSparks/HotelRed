using Godot;
using System;

public class RecRoom : Node2D
{
	[Export]
	private NodePath npc;

	[Export]
	private NodePath paper;

    public override void _Ready()
    {
        if (Controller.Flag("item_paper") == 1 || Controller.Flag("neftali_cutscene") == 0)
		{
			GetNode<EventNPC>(npc).QueueFree();
			GetNode<WorldItem>(paper).QueueFree();
		}	
    }
}
