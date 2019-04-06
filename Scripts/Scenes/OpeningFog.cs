using Godot;
using System;

public class OpeningFog : Sprite
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    //public override void _Ready()
    //{
    //    
    //}

	public override void _Process(float delta)
	{
		//if (Player.Main.Position.y > 792)
		//{
			float a = Mathf.Clamp(700f / Player.Main.Position.y, 0f, 0.85f);
			Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, a);
		//}
	}
}
