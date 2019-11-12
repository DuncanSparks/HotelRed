using Godot;
using System;

public class L1_Balcony_Hallway : Node2D
{
    [Export]
    private NodePath ev;

    public override void _Ready()
    {
        if (Controller.Flag("item_keycard1") == 0)
            GetNode<Event>(ev).QueueFree();
    }
}
