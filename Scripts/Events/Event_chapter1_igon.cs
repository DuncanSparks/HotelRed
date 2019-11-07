using Godot;
using System;

public class Event_chapter1_igon : AnimationPlayer
{
    [Export(PropertyHint.File, "*.txt")]
    private string dialogueFile = string.Empty;

    [Export]
	private SpriteFrames raviaFrames;

    [Export]
    private SpriteFrames igonFrames;
    
    // ================================================================

    private void Resume()
	{
		GetParent<Event>().ResumeEvent();
	}

    // ================================================================

    public void Event_Dialogue(int set)
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, set, "Ravia", "#2391ef",  raviaFrames, "Igon", "#880000", igonFrames, false, this, "Resume");
	}


	public void Event_DialogueSingle(int set)
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, set, "Ravia", "#2391ef", raviaFrames, restoreMovement: false, signalConnection: this, signalMethod: "Resume");
	}


    public void Event_RaviaTurn(int direction)
	{
		Player.Face = (Player.SpriteDirection)direction;
	}


    public void Event_RaviaBubble(int type)
	{
		Controller.ShowBubble((Controller.BubbleType)type, Player.BubblePosition);
	}
}
