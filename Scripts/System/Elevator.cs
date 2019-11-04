using Godot;
using System;


public class Elevator : KinematicBody2D
{
    [Export]
	private string npcName = string.Empty;

	[Export]
	private Color npcColor = new Color(1, 1, 1);

	[Export]
	private SpriteFrames npcSprite;

	[Export]
	private SpriteFrames npcPortrait;

	[Export]
	private int maxDialogueSet;

	[Export]
	private bool autoAdvanceSet;

	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile = string.Empty;

    [Export]
    private int elevatorNumber;

    private int dialogueSet = 0;
	private string npcColorStr;
	private SpriteFrames raviaPortrait = GD.Load<SpriteFrames>("res://Resources/Portrait Sets/Portraits_Ravia.tres");

    private bool disabled = false;

	private Sprite interact;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		interact = GetNode<Sprite>("Interact");
		npcColorStr = $"#{npcColor.ToArgb32().ToString("X").Substring(2)}";

		interact.Hide();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!disabled)
		{
			ZIndex = (int)Position.y;

			if (Input.IsActionJustPressed("sys_accept") && Player.State == Player.ST.MOVE && interact.Visible)
			{
				Player.State = Player.ST.NO_INPUT;
				interact.Hide();
                // if(Player.itemsCollected[elevatorNumber+2])
                // {
                    dialogueSet = 1;
                    GetNode<CollisionShape2D>("CollisionArea").SetDisabled(true);
                // }
				Controller.Dialogue(dialogueFile, dialogueSet, "Ravia", "#2391ef",  raviaPortrait, npcName, npcColorStr, npcPortrait, signalConnection: this, signalMethod: "EndDialogue");
			}
		}
    }

    private void EndDialogue()
	{
		Player.State = Player.ST.MOVE;
		interact.Show();
		dialogueSet = Mathf.Min(dialogueSet + 1, maxDialogueSet);
	}


	private void InteractAreaEntered(Area2D area)
	{
		if (!disabled && area.IsInGroup("PlayerSight") && Player.State == Player.ST.MOVE)
		{
			interact.Show();
		}
	}


	private void InteractAreaExited(Area2D area)
	{
		if (!disabled && area.IsInGroup("PlayerSight"))
		{
			interact.Hide();
		}
	}
}
