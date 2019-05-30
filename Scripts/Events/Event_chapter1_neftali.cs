using Godot;
using System;

public class Event_chapter1_neftali : AnimationPlayer
{
	[Export]
	private SpriteFrames raviaFrames;

	[Export]
	private SpriteFrames neftaliFrames;

	[Export(PropertyHint.File, "*.txt")]
	private string dialogueFile = string.Empty;

	[Export]
	private NodePath neftaliInstance;

	[Export]
	private AudioStream neftaliMusic;

	[Export]
	private AudioStream soulStealSound1;

	[Export]
	private PackedScene soulParts1;

	[Export]
	private PackedScene neftaliFace1;

	[Export]
	private PackedScene partsBackground;

	[Export]
	private PackedScene partsEnd;

	[Export]
	private AudioStream soulParts1Sound;

	[Export]
	private AudioStream soulStealSound2;

	[Export]
	private AudioStream soulStealSound3;

	[Export]
	private AudioStream soulStealSound4;

	[Export]
	private AudioStream soulStealSoundFinal;

	[Export]
	private AudioStream afterMusic;

	// Instance refs
	private Particles2D parts1;
	private Particles2D parts2;
	private Particles2D parts3;
	private Sprite face1;
	private Sprite face2;

	// Refs
	private EventNPC neftaliNPC;
	private AudioStreamPlayer speaker;

	// ================================================================

	public override void _Ready()
	{
		neftaliNPC = GetNode<EventNPC>(neftaliInstance);//GetNode<EventNPC>(neftaliInstance);
		speaker = GetParent<Area2D>().GetNode<AudioStreamPlayer>("Speaker");
	}

	// ================================================================

	private void Resume()
	{
		GetParent<Event>().ResumeEvent();
	}

	// ================================================================

	public void Event_FadeMusic()
	{
		Controller.FadeMusic(0.5f);
	}


	public void Event_PlayNeftaliMusic()
	{
		Controller.PlayCharacterTheme(neftaliMusic);
	}


	public void Event_Dialogue(int set)
	{
		GetParent<Event>().PauseEvent();
		Controller.Dialogue(dialogueFile, set, "Ravia", "#2391ef",  raviaFrames, "Neftali", "#ff0000", neftaliFrames, false, this, "Resume");
	}


	public void Event_RaviaTurn(int direction)
	{
		Player.Face = (Player.SpriteDirection)direction;
	}


	public void Event_NeftaliTurn(int direction)
	{
		neftaliNPC.Face = (EventNPC.SpriteDirection)direction;
	}


	public void Event_NeftaliWalk(float speed, int motionX, int motionY, int direction)
	{
		neftaliNPC.WalkSpeed = speed;
		neftaliNPC.Motion = new Vector2(motionX, motionY);
		neftaliNPC.Face = (EventNPC.SpriteDirection)direction;
		neftaliNPC.Walking = true;
	}


	public void Event_RaviaWalk(float speed, int motionX, int motionY, int direction)
	{
		Player.Walking = true;
		Player.MotionOverrideVec = new Vector2(motionX, motionY);
		Player.Face = (Player.SpriteDirection)direction;
		Player.MotionOverride = true;
		Player.WalkSpeedOverride = speed;
	}


	public void Event_RaviaStop()
	{
		Player.Walking = false;
		Player.MotionOverrideVec = new Vector2(0, 0);
	}


	public void Event_RaviaBubble(int type)
	{
		Controller.ShowBubble((Controller.BubbleType)type, Player.BubblePosition);
	}


	public void Event_NeftaliStop()
	{
		neftaliNPC.Motion = new Vector2(0, 0);
		neftaliNPC.WalkSpeed = 0f;
		neftaliNPC.Walking = false;
	}


	public void Event_BothWalkLeft()
	{
		Event_NeftaliWalk(180f, -1, 0, 2);
		Player.Walking = true;
		Player.MotionOverrideVec = new Vector2(-1, 0);
		Player.Face = Player.SpriteDirection.LEFT;
		Player.MotionOverride = true;
		Player.WalkSpeedOverride = 180f;
	}


	public void Event_SoulFadeIn()
	{
		Player.DepthControl = false;
		Player.Main.ZIndex = 1600;

		neftaliNPC.DepthControl = false;
		neftaliNPC.ZIndex = 1600;
		speaker.Stream = soulStealSound1;
		speaker.Play(0.8f);
	}


	public void Event_FadeCharTheme()
	{
		Controller.FadeCharacterTheme(3.5f);
	}


	public void Event_SoulFadeOut()
	{
		neftaliNPC.Hide();
		Controller.PlaySoundBurst(soulStealSound2);
	}


	public void Event_SoulParts1Appear()
	{
		Controller.PlaySoundBurst(soulStealSound3);
		Controller.PlaySoundBurst(soulStealSound4, pitch: 0.6f);
		//var parts = (Particles2D)soulParts1.Instance();
		parts1 = (Particles2D)soulParts1.Instance();
		parts1.Position = new Vector2(Player.Main.Position.x, Player.Main.Position.y + 36);
		//parts.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Close in").TrackSetKeyValue(2, 0, new Vector2(Player.Main.Position.x, Player.Main.Position.y + 74));
		//parts.GetNode<AnimationPlayer>("AnimationPlayer").GetAnimation("Close in").TrackSetKeyValue(2, 1, new Vector2(Player.Main.Position.x, Player.Main.Position.y + 74 - 56));
		GetTree().GetRoot().AddChild(parts1);
	}


	public void Event_SoulParts1Sound()
	{
		Controller.PlaySoundBurst(soulStealSound4);
	}


	public void Event_Particles2()
	{
		parts2 = (Particles2D)partsBackground.Instance();
		//parts.Position = Player.Main.Position;
		//GetTree().GetRoot().AddChild(parts);
		Player.Main.AddChild(parts2);
	}


	public void Event_NeftaliFace1()
	{
		face1 = (Sprite)neftaliFace1.Instance();
		face1.GetNode<AnimationPlayer>("AnimationPlayer").Play("Fade 2");
		face1.Position = new Vector2(Player.Main.Position.x - 220, Player.Main.Position.y + 100);
		GetTree().GetRoot().AddChild(face1);
	}


	public void Event_NeftaliFace2()
	{
		face2 = (Sprite)neftaliFace1.Instance();
		face2.GetNode<AnimationPlayer>("AnimationPlayer").Play("Fade 1");
		face2.Position = new Vector2(Player.Main.Position.x + 200, Player.Main.Position.y - 80);
		//var mat = (ShaderMaterial)face.Material;
		//mat.SetShaderParam("sinConst", -0.3f);
		GetTree().GetRoot().AddChild(face2);
		parts3 = (Particles2D)partsEnd.Instance();
		Player.Main.AddChild(parts3);
	}


	public void Event_AnimationEnd()
	{
		//var parts = (Particles2D)partsEnd.Instance();
		//Player.Main.AddChild(parts);
		Controller.PlaySoundBurst(soulStealSoundFinal);
	}


	public void Event_AnimationCleanup()
	{
		parts1.QueueFree();
		parts2.QueueFree();
		parts3.QueueFree();
		face1.QueueFree();
		face2.QueueFree();
		Player.CurrentSpriteSet = Player.SpriteSet.KNEEL;
		neftaliNPC.Show();
	}
}
