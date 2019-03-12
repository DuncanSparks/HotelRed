using Godot;
using System;

public class TitleScreen : Control
{
    [Export]
	private bool UseTitleScreen = true;

	[Export]
	private PackedScene StartScene;

	// ================================================================

    public override void _Ready()
    {
        if (!UseTitleScreen)
			Controller.SceneGoto(StartScene);
    }


	public void ClickNewGame()
	{
		Player.EnableCamera(true);
		Controller.SceneGoto(StartScene);
	}
}
