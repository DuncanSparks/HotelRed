using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Controller : Node
{
	private static Controller inst = null;
	private static Controller Main { get { return inst; } }

	Controller()
	{
		if (inst == null)
			inst = this;
		else
			QueueFree();
	}

	// ================================================================

	private Dictionary<string, int> flag = new Dictionary<string, int>()
	{
		{"item_paper", 0},

		{"npc_cashier", 0},
		{"npc_waterperson", 0},

		{"enter_foyer", 0},
		{"neftali_cutscene", 0},
	};

	private int doorCode = 0;

	public enum Sound {HOVER, SELECT, BUBBLE_EXCLAMATION};
	public enum BubbleType {EXCLAMATION, QUESTION, SILENCE};

	private AudioStream currentMusic = null;
	private AudioStream currentAmbience = null;
	private AudioStream currentCharacterTheme = null;

	private static Regex loadRegex = new Regex(@"^(.*) (\d*)$");

	// Refs
	private PackedScene DialogueRef = GD.Load<PackedScene>("res://Instances/System/Dialogue.tscn");
	private PackedScene SoundBurstRef = GD.Load<PackedScene>("res://Instances/System/SoundBurst.tscn");
	private PackedScene BubbleRef = GD.Load<PackedScene>("res://Instances/System/Bubble.tscn");

	private Timer TimerEndTransitionRef;
	private Timer TimerEndTransition2Ref;

	// Sounds
	private AudioStreamPlayer SoundSysHover;
	private AudioStreamPlayer SoundSysSelect;
	private AudioStreamPlayer SoundSysBubbleExclamation;

	// ================================================================

	public static AudioStream CurrentMusic { get { return Controller.Main.currentMusic; } set { Controller.Main.currentMusic = value; } }
	public static AudioStream CurrentAmbience { get { return Controller.Main.currentAmbience; } set { Controller.Main.currentAmbience = value; } }
	public static AudioStream CurrentCharacterTheme { get { return Controller.Main.currentCharacterTheme; } set { Controller.Main.currentCharacterTheme = value; } }
	public static int DoorCode { get => Controller.Main.doorCode; }

	// ================================================================

	public override void _Ready()
	{
		// Refs
		SoundSysHover = GetNode<AudioStreamPlayer>("SoundSysHover");
		SoundSysSelect = GetNode<AudioStreamPlayer>("SoundSysSelect");
		SoundSysBubbleExclamation = GetNode<AudioStreamPlayer>("SoundSysBubbleExclamation");

		TimerEndTransitionRef = GetNode<Timer>("TimerEndTransition");
		TimerEndTransition2Ref = GetNode<Timer>("TimerEndTransition2");
    
		// Other stuff
		GD.Randomize();

		doorCode = Mathf.RoundToInt((float)GD.RandRange(0, 9999));
	}

	// ================================================================

	public static int Flag(string flag)
	{
		return Controller.Main.flag[flag];
	}


	public static void SetFlag(string flag, int value)
	{
		Controller.Main.flag[flag] = value;
	}


	public static SceneTag GetSceneTag()
	{
		return Controller.Main.GetTree().GetRoot().GetNode<Node2D>("Scene").GetNode<SceneTag>("SceneTag");
	}


	public static void SceneGoto(PackedScene targetScene)
	{
		Controller.Main.SceneGotoPre();
		Controller.Main.GetTree().ChangeSceneTo(targetScene);
		Controller.Main.GetNode<Timer>("TimerSceneGoto").Start();
	}
	

	public static void SceneGoto(string targetScene)
	{
		Controller.Main.SceneGotoPre();
		Controller.Main.GetTree().ChangeScene(targetScene);
		Controller.Main.GetNode<Timer>("TimerSceneGoto").Start();
	}


	public static void PlaySoundBurst(AudioStream sound, float volume = 0f, float pitch = 1f)
	{
		var sb = Controller.Main.SoundBurstRef.Instance() as AudioStreamPlayer;
		sb.Stream = sound;
		sb.VolumeDb = volume;
		sb.PitchScale = pitch;
		Controller.Main.GetTree().GetRoot().AddChild(sb);
		sb.Play();
	}


	public static void PlaySystemSound(Sound sound)
	{
		switch (sound)
		{
			case Controller.Sound.HOVER:
				PlaySoundBurst(Controller.Main.SoundSysHover.Stream, volume: -8, pitch: 1.1f);
				break;
			case Controller.Sound.SELECT:
				PlaySoundBurst(Controller.Main.SoundSysSelect.Stream);
				break;
			case Controller.Sound.BUBBLE_EXCLAMATION:
				PlaySoundBurst(Controller.Main.SoundSysBubbleExclamation.Stream);
				break;
		}
	}


	public static void ShowBubble(BubbleType type, Vector2 position)
	{
		var bubble = Controller.Main.BubbleRef.Instance() as Bubble;
		switch (type)
		{
			case BubbleType.EXCLAMATION:
			{
				bubble.PlayAnimation("Exclamation");
				Controller.PlaySystemSound(Controller.Sound.BUBBLE_EXCLAMATION);
				break;
			}
			
			case BubbleType.QUESTION:
				bubble.PlayAnimation("Question");
				break;
			case BubbleType.SILENCE:
				bubble.PlayAnimation("Silence");
				break;
			default:
				bubble.PlayAnimation("Exclamation");
				break;
		}

		bubble.Position = position;
		Controller.Main.GetTree().GetRoot().AddChild(bubble);
	}


	public static void Dialogue(string sourceFile, int dialogueSet, string leftClientName, string leftClientColor, SpriteFrames leftClientPortrait, string rightClientName = "NULL", string rightClientColor = "#ffffff", SpriteFrames rightClientPortrait = null, bool restoreMovement = true, Node signalConnection = null, string signalMethod = "")
	{
		Player.State = Player.ST.NO_INPUT;
		var dlg = Controller.Main.DialogueRef.Instance() as Dialogue;
		dlg.SourceFile = sourceFile;
		dlg.TextSet = dialogueSet;
		dlg.SecondClient = rightClientPortrait != null;

		dlg.LeftClientName = leftClientName;
		dlg.LeftClientColor = new Color(leftClientColor);
		dlg.LeftClientPortrait = leftClientPortrait;

		dlg.RightClientName = rightClientName;
		dlg.RightClientColor = new Color(rightClientColor);
		dlg.RightClientPortrait = rightClientPortrait;

		dlg.RestoreMovement = restoreMovement;

		if (rightClientPortrait != null)
		{
			dlg.TextLeft -= 2;
			dlg.LineEnd = 230;

			// I'm really sorry about this
			// Shift box positions (start)
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(0, 0, new Vector2(335 - 36, 424));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(0, 1, new Vector2(335 - 36, 284));
			
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(1, 0, new Vector2(83 - 36, 368));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(1, 1, new Vector2(83 - 36, 228));
		
			// Shift box positions (finish)
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(0, 0, new Vector2(335 - 36, 284));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(0, 1, new Vector2(335 - 36, 424));
			
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(1, 0, new Vector2(83 - 36, 228));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(1, 1, new Vector2(83 - 36, 368));
		}
		else
		{
			dlg.TextLeft += 35;

			// Shift box positions (start)
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(0, 0, new Vector2(335, 424));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(0, 1, new Vector2(335, 284));
			
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(1, 0, new Vector2(83, 368));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Start").TrackSetKeyValue(1, 1, new Vector2(83, 228));
		
			// Shift box positions (finish)
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(0, 0, new Vector2(335, 284));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(0, 1, new Vector2(335, 424));
			
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(1, 0, new Vector2(83, 228));
			dlg.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Finish").TrackSetKeyValue(1, 1, new Vector2(83, 368));
		}
			
		dlg.Position = new Vector2(Player.GetCamera().GetCameraScreenCenter().x - 300, Player.GetCamera().GetCameraScreenCenter().y - 180);
		Controller.Main.GetTree().GetRoot().AddChild(dlg);
		dlg.InitializePortraits();

		if (signalConnection != null && signalMethod != "")
			dlg.Connect("text_ended", signalConnection, signalMethod);
	}


	public static void Fade(bool fadein, bool white, float time)
	{
		Controller.Main.GetNode<CanvasLayer>("CanvasLayer").GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 1f / time;
		if (white)
			Controller.Main.GetNode<CanvasLayer>("CanvasLayer").GetNode<AnimationPlayer>("AnimationPlayer").Play(fadein ? "Fadein white" : "Fadeout white");
		else
			Controller.Main.GetNode<CanvasLayer>("CanvasLayer").GetNode<AnimationPlayer>("AnimationPlayer").Play(fadein ? "Fadein black" : "Fadeout black");
	}
	
	public static void SetMusic(AudioStream music)
	{
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").Stream = music;
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").VolumeDb = 0;
	}

	public static void PlayMusic(AudioStream music)
	{
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").Stream = music;
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").VolumeDb = 0;
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").Play();
	}


	public static void FadeMusic(float time)
	{
		Controller.Main.GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 1f / time;
		Controller.Main.GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeMusic");
	}
	
	public static void FadeInMusic(float time)
	{
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").Play();
		Controller.Main.GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 1f / time;
		Controller.Main.GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeIn");	
	}

	
	public static void StopMusic()
	{
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").Stop();
	}


	public static void PlayAmbience(AudioStream ambience)
	{
		Controller.Main.GetNode<AudioStreamPlayer>("AMBIENCE").Stream = ambience;
		Controller.Main.GetNode<AudioStreamPlayer>("AMBIENCE").Play();
	}


	public static void StopAmbience()
	{
		Controller.Main.GetNode<AudioStreamPlayer>("AMBIENCE").Stop();
	}
	

	public static void PlayCharacterTheme(AudioStream characterTheme)
	{
		//Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").VolumeDb = -60;
		Controller.Main.GetNode<AudioStreamPlayer>("CHARACTERTHEME").Stream = characterTheme;
		Controller.Main.GetNode<AudioStreamPlayer>("CHARACTERTHEME").Play();
	}


	public static void FadeCharacterTheme(float time)
	{
		Controller.Main.GetNode<AnimationPlayer>("AnimationPlayer").PlaybackSpeed = 1f / time;
		Controller.Main.GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeCharTheme");
	}

	
	public static void StopCharacterTheme()
	{
		Controller.Main.GetNode<AudioStreamPlayer>("CHARACTERTHEME").Stop();
		Controller.Main.GetNode<AudioStreamPlayer>("MUSIC").VolumeDb = 0;
	}


	public static void EndTransition()
	{
		Controller.Main.TimerEndTransitionRef.Start();
		Fade(true, false, 0.25f);
	}


	public static void ShowSceneName(string name)
	{
		Controller.Main.GetNode<CanvasLayer>("CanvasLayer").GetNode<Label>("Label").Text = name;
		Controller.Main.GetNode<CanvasLayer>("CanvasLayer").GetNode<AnimationPlayer>("AnimationPlayer2").Play("Text In");
	}


	public static void SaveGame()
	{
		File saveFile = new File();
		try
		{
			saveFile.OpenEncryptedWithPass("user://data.hr", (int)File.ModeFlags.Write, OS.GetUniqueId());

			// Player position
			saveFile.StoreLine($"{Player.Main.Position.x} {Player.Main.Position.y}");

			// Flags
			foreach (var pair in Controller.Main.flag)
				saveFile.StoreLine($"{pair.Key} {pair.Value}");
		}
		finally
		{
			if (saveFile.IsOpen())
				saveFile.Close();
		}
	}


	public static void LoadGame()
	{
		File loadFile = new File();
		try
		{
			loadFile.OpenEncryptedWithPass("user://data.hr", (int)File.ModeFlags.Read, OS.GetUniqueId());
			while (!loadFile.EofReached())
			{
				string line = loadFile.GetLine();
				string key = Controller.loadRegex.Match(line).Groups[1].ToString();
				int value = Controller.loadRegex.Match(line).Groups[2].ToString().ToInt();
				Controller.SetFlag(key, value);
			}
		}
		catch
		{
			GD.Print("FILE NOT FOUND");
		}
		finally
		{
			if (loadFile.IsOpen())
				loadFile.Close();
		}
	}

	// ================================================================

	private void SceneGotoPre()
	{

	}


	private void SceneGotoPost()
	{
		Player.GetCamera().LimitRight = GetTree().GetRoot().GetNode<Node2D>("Scene").GetNode<SceneTag>("SceneTag").CameraLimitRight;
		Player.GetCamera().LimitBottom = GetTree().GetRoot().GetNode<Node2D>("Scene").GetNode<SceneTag>("SceneTag").CameraLimitBottom;
	}


	private void EndTransition2()
	{
		Controller.Main.TimerEndTransition2Ref.Start();
	}


	private void EndTransition3()
	{
		Player.MotionOverrideVec = new Vector2(0, 0);
		Player.MotionOverride = false;
		Player.Motion = new Vector2(0, 0);
		Player.Walking = false;
		Player.State = Player.ST.MOVE;
		Player.Teleporting = false;
	}
}
