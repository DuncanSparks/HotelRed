using Godot;
using System;

public class L1_Foyer : Node2D
{
	public override void _Ready()
	{
		if (Controller.Flag("enter_foyer") == 1)
			GetNode<Node2D>("SoulFade2").GetNode<ColorRect>("ColorRect").Color = new Color(1, 0, 0, 0);

		if (Controller.Flag("neftali_cutscene") == 1)
			GetNode<EventNPC>("EventNPCNeftali").QueueFree();
		else
			GetNode<NPC>("NPCNeftali").Disabled = true;
	}
}