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

    [Export]
    private AudioStream doorSound;

    [Export]
    private AudioStream elevatorSound;

    public void Event_DoorClose()
	{	
		Controller.PlaySoundBurst(doorSound);
        GetTree().GetRoot().GetNode<Sprite>("Closed").SetVisible(true);
        GetTree().GetRoot().GetNode<Sprite>("Closed").SetZIndex(101);
	}

    public void Event_DoorOpen()
	{	
		Controller.PlaySoundBurst(doorSound);
        GetTree().GetRoot().GetNode<Sprite>("Closed").SetVisible(false);
        GetTree().GetRoot().GetNode<Sprite>("Open").SetVisible(true);
	}

    public void Event_ElevatorSound()
    {
        Controller.PlaySoundBurst(elevatorSound);
    }

    public void Event_Transition()
	{
		Player.SpriteOverride = false;
		Player.State = Player.ST.MOVE;
		Controller.SceneGoto(targetScene);
		Player.Main.Position = targetPosition;
	} 
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
