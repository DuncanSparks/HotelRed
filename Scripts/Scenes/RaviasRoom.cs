using Godot;
using System;

public class RaviasRoom : Node2D
{
    // Refs
	private AudioStream music; 
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        music = GetNode<AudioStreamPlayer>("Music").GetStream();
		Controller.SetMusic(music);
		Controller.FadeInMusic(1);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
