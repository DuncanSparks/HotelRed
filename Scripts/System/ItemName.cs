using Godot;
using System;

public class ItemName : Node2D
{
    public override void _Ready()
    {
        GetNode<Particles2D>("Particles2D").Emitting = true;
    }


	public void SetItemName(string name)
	{
		GetNode<Label>("Text").Text = name;
	}
}
