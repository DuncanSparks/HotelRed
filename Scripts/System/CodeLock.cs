using Godot;
using System;


public class CodeLock : KinematicBody2D
{
	// Refs
   // [Export]
    private string doorCode;

    [Export]
    private int xScale;

    [Export]
    private int yScale;

	//private Sprite interact;
	private bool inArea = false;
    private bool isOpen = false;
    private int current = 0;
    private static int[] digitList = {-1, -1, -1, -1};
	private AudioStream keyPadSound;
    private AudioStream UnlockedDoorSound;
    private AudioStream WrongCodeSound;
    private int ownerID;

	// ================================================================

	public override void _Ready()
	{
		//interact = GetNode<Sprite>("Interact");
		keyPadSound = GetNode<AudioStreamPlayer>("KeyPadSound").GetStream();
        UnlockedDoorSound = GetNode<AudioStreamPlayer>("UnlockedDoorSound").GetStream();
        WrongCodeSound = GetNode<AudioStreamPlayer>("WrongCodeSound").GetStream();
		//interact.Hide();
        GetNode<CollisionShape2D>("CollisionArea").SetScale(new Vector2(xScale, yScale));
        GetNode<Area2D>("InteractionArea").SetScale(new Vector2(xScale, yScale));

		doorCode = Controller.DoorCode.ToString();

        if (Controller.Flag("unlock_office") == 1)
		{
			GetNode<CollisionShape2D>("CollisionArea").SetDisabled(true);
			GetNode<Sprite>("Keypad").Frame = 1;
		}
            
	}


	public override void _Process(float delta)
	{
		ZIndex = (int)Position.y;
        if (isOpen)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                clearCells();
                Player.State = Player.ST.MOVE;
                isOpen = false;
                Player.InventoryLock = false;
                Player.CodeOverlay.SetVisible(false);
            }
            else if (current == 4)
            {
                if(isCorrectCode())
                {
                    GetNode<CollisionShape2D>("CollisionArea").SetDisabled(true);
					GetNode<Sprite>("Keypad").Frame = 1;
                    Controller.PlaySoundBurst(UnlockedDoorSound);
                    Controller.SetFlag("unlock_office", 1);
					inArea = false;
					//Player.ShowInteract(true);
					//Player.ShowInteract(false);
                }
                else
				{
					Controller.PlaySoundBurst(WrongCodeSound);
					Player.ShowInteract(true);
				}
                
                Player.State = Player.ST.MOVE;
                isOpen = false;
                Player.InventoryLock = false;
                Player.CodeOverlay.SetVisible(false);
                clearCells();
            }
            else if (Input.IsActionJustPressed("Backspace"))
                backSpace();
            else if (Input.IsActionJustPressed("Zero"))
                setCell(0);
            else if (Input.IsActionJustPressed("One"))
                setCell(1);
            else if (Input.IsActionJustPressed("Two"))
                setCell(2);
            else if (Input.IsActionJustPressed("Three"))
                setCell(3);
            else if (Input.IsActionJustPressed("Four"))
                setCell(4);
            else if (Input.IsActionJustPressed("Five"))
                setCell(5);
            else if (Input.IsActionJustPressed("Six"))
                setCell(6);
            else if (Input.IsActionJustPressed("Seven"))
                setCell(7);
            else if (Input.IsActionJustPressed("Eight"))
                setCell(8);
            else if (Input.IsActionJustPressed("Nine"))
                setCell(9);
			
        }
		else if (Input.IsActionJustPressed("sys_accept") && Player.State == Player.ST.MOVE && inArea)
		{
			Player.State = Player.ST.NO_INPUT;
			Player.ShowInteract(false);
			//interact.Hide();
            isOpen = true;
            Player.InventoryLock = true;
            Player.CodeOverlay.SetVisible(true);
        }
        
    }
	// ================================================================

    private bool isCorrectCode()
    {
       // for(auto & d: doorCode)
        for (int i = 0; i < 4; i++)
        {
            string temp = doorCode[i].ToString();
            if(temp != digitList[i].ToString())
            {
                return false;
            }
        }
        return true;
    }


	private void InteractAreaEntered(Area2D area)
	{
		if (area.IsInGroup("PlayerSight") && Player.State == Player.ST.MOVE && Controller.Flag("unlock_office") == 0)
		{
			inArea = true;
			Player.ShowInteract(true);
			//interact.Show();
		}
	}


	private void InteractAreaExited(Area2D area)
	{
		if (area.IsInGroup("PlayerSight"))
		{
			inArea = false;
			Player.ShowInteract(false);
			//interact.Hide();
		}
	}


    private void setCell(int digit)
    {
        digitList[current] = digit;
        Player.CodeOverlay.GetChild<Control>(current+1).GetChild<Sprite>(digit).SetVisible(true);
        current++;
		Controller.PlaySoundBurst(keyPadSound);
    }


    private void backSpace()
    {
        if (current != 0)
        {
           Player.CodeOverlay.GetChild<Control>(current).GetChild<Sprite>(digitList[current-1]).SetVisible(false);
           current--;
        }
		Controller.PlaySoundBurst(keyPadSound);
    }


    private void clearCells()
    {
        for (int i = 0; i < current; i++)
        {
            Player.CodeOverlay.GetChild<Control>(i+1).GetChild<Sprite>(digitList[i]).SetVisible(false);
        }
        current = 0;
    }
}
