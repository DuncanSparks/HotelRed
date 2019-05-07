using Godot;
using System;

public class SceneTag : Label
{
    [Export]
	private string sceneName = string.Empty;

	[Export]
	private bool displayName = true;

	[Export]
	private AudioStream sceneMusic;

	[Export]
	private int cameraLimitRight = 600;

	[Export]
	private int cameraLimitBottom = 360;

	// ================================================================

	public int CameraLimitRight { get { return cameraLimitRight; } }
	public int CameraLimitBottom { get { return cameraLimitBottom; } }

	// ================================================================

	public override void _Ready()
	{
		if (sceneMusic != Controller.CurrentMusic)
		{
			Controller.PlayMusic(sceneMusic);
			Controller.CurrentMusic = sceneMusic;
		}

		/* if (sceneAmbience != Controller.currentAmbience)
		{
			Controller.PlayAmbience(sceneAmbience);
			Controller.CurrentAmbience = sceneAmbience;
		}*/
	}
}
