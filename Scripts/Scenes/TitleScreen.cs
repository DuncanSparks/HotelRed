using Godot;
using System;

public class TitleScreen : Control
{
    [Export]
	private bool UseTitleScreen = true;

	[Export]
	private PackedScene StartScene;

	[Export]
	private Vector2 StartPosition = new Vector2(0, 185);

	[Export]
	private float shaderWaveConst = 0.1f;

	[Export]
	private float ShaderWaveSpeed = 5f;

	[Export]
	private AudioStream thunderclapSound;

	private bool passToShader = true;

	// Refs
	private ShaderMaterial Shader;

	// ================================================================

    public override void _Ready()
    {
        if (!UseTitleScreen)
		{
			Player.EnableCamera(true);
			Controller.SceneGoto(StartScene);
			Player.Main.Position = StartPosition;
		}
		

		Shader = (ShaderMaterial)GetNode<CanvasLayer>("CanvasLayer").GetNode<ColorRect>("Shader").Material;
	}


	public override void _Process(float delta)
	{
		if (passToShader)
		{
			Shader.SetShaderParam("waveConst", shaderWaveConst);
			Shader.SetShaderParam("speed", ShaderWaveSpeed);
		}
	}

	// ================================================================

	public void ButtonHover()
	{
		Controller.PlaySystemSound(Controller.Sound.HOVER);
	}


	public void ClickNewGame()
	{
		/* Controller.PlaySystemSound(Controller.Sound.SELECT);
		Player.EnableCamera(true);
		Controller.SceneGoto(StartScene);
		Player.Main.Position = StartPosition; */
		Controller.PlaySystemSound(Controller.Sound.SELECT);
		GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 0.6f;
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Fadeout");
	}


	public void ClickExit()
	{
		Controller.PlaySystemSound(Controller.Sound.SELECT);
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Exit");
	}

	// ================================================================

	private void TimerIntroTimeout()
	{
		Controller.Fade(false, false, 0.1f);
		Controller.PlaySoundBurst(thunderclapSound);
	}


	private void TimerIntroTimeout2()
	{
		Player.EnableCamera(true);
		Controller.SceneGoto(StartScene);
		Player.Main.Position = StartPosition;
	}


	private void AnimationFinished(string anim_name)
	{
		switch (anim_name)
		{
			case "Fadein":
			{
				passToShader = false;
				GetNode<CanvasLayer>("CanvasLayer").GetNode<ColorRect>("Shader").QueueFree();
				GetNode<AnimationPlayer>("AnimationPlayer").Play("Fadein 2");
				break;
			}

			case "Fadeout":
			{
				GetNode<Timer>("TimerIntro").Start();
				GetNode<Timer>("TimerIntro2").Start();
				break;
			}

			case "Exit":
				GetTree().Quit();
				break;
		}
	}
}
