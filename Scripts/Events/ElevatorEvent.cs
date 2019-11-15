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
    private AudioStream closeSound;

    [Export]
    private AudioStream openSound;

    [Export]
    private AudioStream elevatorSound;

    public void Event_DoorClose()
	{	
		Controller.PlaySoundBurst(closeSound);
        GetTree().GetRoot().GetNode<Sprite>("Closed").SetVisible(true);
        GetTree().GetRoot().GetNode<Sprite>("Closed").SetZIndex(101);
	}

    public void Event_DoorOpen()
	{	
		Controller.PlaySoundBurst(openSound);
        GetTree().GetRoot().GetNode<Sprite>("Closed").SetVisible(false);
        GetTree().GetRoot().GetNode<Sprite>("Open").SetVisible(true);
	}

    public void Event_ElevatorSound()
    {
        Controller.PlaySoundBurst(elevatorSound);
    }


//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
