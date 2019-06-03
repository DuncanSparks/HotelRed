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
	private bool inCredits = false;
	private bool buffer = false;

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
			Player.State = Player.ST.MOVE;
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

		if (Input.IsActionJustPressed("sys_accept") && inCredits && !buffer)
		{
			inCredits = false;
			GetNode<AnimationPlayer>("AnimationPlayerCredits").Play("Credits fade");
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Credits back");
			buffer = true;
			GetNode<Timer>("TimerBuffer").Start();
		}
	}

	// ================================================================

	public void ButtonHover()
	{
		Controller.PlaySystemSound(Controller.Sound.HOVER);
	}


	public void ClickNewGame()
	{
		if (!buffer)
		{
			Controller.PlaySystemSound(Controller.Sound.SELECT);
			GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 0.6f;
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Fadeout");
		}
	}


	public void ClickCredits()
	{
		if (!buffer)
		{
			Controller.PlaySystemSound(Controller.Sound.SELECT);
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Fadeout 2");
			GetNode<AnimationPlayer>("AnimationPlayerCredits").Play("Credits roll");
			inCredits = true;
		}
	}


	public void ClickExit()
	{
		if (!buffer)
		{
			Controller.PlaySystemSound(Controller.Sound.SELECT);
			GetNode<AnimationPlayer>("AnimationPlayer").Play("Exit");
		}
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
		Player.State = Player.ST.MOVE;
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


	private void ResetBuffer()
	{
		buffer = false;
	}
}
