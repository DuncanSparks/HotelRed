using Godot;
using System;

public class ElevatorEvent : AnimationPlayer
{
    [Export]
    private AudioStream newMusic;

    [Export(PropertyHint.File, "*.tscn")]
	private string targetScene = string.Empty;

    [Export]
    private Vector2 targetPosition;


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
