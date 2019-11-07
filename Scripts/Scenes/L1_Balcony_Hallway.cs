using Godot;
using System;

public class L1_Balcony_Hallway : Node2D
{
    [Export]
    private NodePath scene;
    public override void _Ready()
    {
        if (Controller.Flag("get_keycard") == 0)
            GetNode<Event>(scene).QueueFree();
    }
}
