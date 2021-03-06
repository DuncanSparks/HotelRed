using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Dialogue : Node2D
{
	[Signal]
	public delegate void text_ended();

	// ================================================================

	[Export]
	private DynamicFont font;

	[Export(PropertyHint.Range, "0, 1")]
	private float textAlpha = 1;

	// ================================================================

	// Text storage
	private List<string> text = new List<string>();
	private List<bool> clients = new List<bool>();
	private List<string> clientExpressionsLeft = new List<string>();
	private List<string> clientExpressionsRight = new List<string>();
	private int textPage = 0;

	private string textOverride = "";

	// File info
	private string sourceFile;
	private int textSet;

	// Display info
	private int textSize = 16;
	private int lineEnd = 280;

	private int disp = -1;
	private bool roll = false;
	private float interval = 0.03f;

	private bool secondClient = false;
	private string leftClientName;
	private string rightClientName;
	private Color leftClientColor;
	private Color rightClientColor;
	private SpriteFrames leftClientPortrait;
	private SpriteFrames rightClientPortrait;
	private bool talkSide = false;

	private int textLeft = 168;

	// Input
	private bool allowAdvance = false;
	private bool buffer = false;

	private bool started = false;
	private bool finished = false;

	private bool restoreMovement = true;

	// Timing
	private float t = 0f;
	private bool pause = false;

	private Node2D client;

	// Constants
	private const int LineSpacing = 18;
	private const int TextTop = 270;
	private const int NametagTop = 252;

	private Regex numberRegex = new Regex(@"\[(\d+)\]");
	private Regex wordRegex = new Regex(@"\b\w*\b");
	private Regex lineRegex = new Regex(@"<\s*([01])\s+(\w+)\s+(\w+)>(.*)");

	// Modifiers
	private enum Modifier {NORMAL, RED, GREEN, BLUE, YELLOW, SHAKE, WAVE};

	// Refs
	private Sprite Indicator;
	private AnimatedSprite PortraitSpriteLeft;
	private AnimatedSprite PortraitSpriteRight;
	private Sprite PortraitFrameLeft;
	private Sprite PortraitFrameRight;
	private AnimationPlayer AnimPlayer;

	private AudioStreamPlayer SoundStart;
	private AudioStreamPlayer SoundType;
	private AudioStreamPlayer SoundAdvance;
	private AudioStreamPlayer SoundEnd;

	private Timer TimerStart;
	private Timer TimerRollText;
	private Timer TimerSound;
	private Timer TimerWaitShort;
	private Timer TimerWaitLong;
	private Timer TimerBuffer;
	private Timer TimerEnd;

	// ================================================================

	public string TextOverride { set => textOverride = value; }
	public int TextLeft { get { return textLeft; } set { textLeft = value; } }
	public int LineEnd { set { lineEnd = value; } }
	public bool SecondClient { set { secondClient = value; } }
	public string LeftClientName { set { leftClientName = value; } }
	public string RightClientName { set { rightClientName = value; } }
	public Color LeftClientColor { set { leftClientColor = value; } }
	public Color RightClientColor { set { rightClientColor = value; } }
	public SpriteFrames LeftClientPortrait { set { leftClientPortrait = value; } }
	public SpriteFrames RightClientPortrait { set { rightClientPortrait = value; } }
	public string SourceFile { set { sourceFile = value;} }
	public int TextSet { set { textSet = value; } }
	public Node2D Client { set { client = value; } }
	public bool Buffer { set { buffer = value; } }
	public bool RestoreMovement { set { restoreMovement = value; } }

	// ================================================================

    public override void _Ready()
    {
		// Refs
		Indicator = GetNode<Sprite>("Indicator");
		PortraitSpriteLeft = GetNode<AnimatedSprite>("BorderLeft/PortraitLeft");
		PortraitSpriteRight = GetNode<AnimatedSprite>("BorderRight/PortraitRight");
		PortraitFrameLeft = GetNode<Sprite>("BorderLeft");
		PortraitFrameRight = GetNode<Sprite>("BorderRight");
		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		SoundStart = GetNode<AudioStreamPlayer>("SoundStart");
		SoundType = GetNode<AudioStreamPlayer>("SoundType");
		SoundAdvance = GetNode<AudioStreamPlayer>("SoundAdvance");
		SoundEnd = GetNode<AudioStreamPlayer>("SoundEnd");

		TimerStart = GetNode<Timer>("TimerStart");
		TimerRollText = GetNode<Timer>("TimerRollText");
		TimerSound = GetNode<Timer>("TimerSound");
		TimerWaitShort = GetNode<Timer>("TimerWaitShort");
		TimerWaitLong = GetNode<Timer>("TimerWaitLong");
		TimerBuffer = GetNode<Timer>("TimerBuffer");
		TimerEnd = GetNode<Timer>("TimerEnd");

		// Set up portraits
		PortraitSpriteLeft.Frames = leftClientPortrait;
		
		if (secondClient)
			PortraitSpriteRight.Frames = rightClientPortrait;
		else
		{
			PortraitFrameRight.Hide();
			PortraitSpriteRight.Hide();
		}

		// Setup
		Indicator.Hide();
		SoundStart.Play();
    }


	public override void _Process(float delta)
	{
		t += 60f * delta;

		if (Input.IsActionJustPressed("sys_accept") && started && !buffer && !finished)
		{
			if (allowAdvance)
				AdvancePage();
			else
			{
				text[textPage] = text[textPage].Replace("|", string.Empty);
				text[textPage] = text[textPage].Replace("{", string.Empty);
				disp = text[textPage].Length - 1;
				roll = false;
				Indicator.Show();
				TimerWaitShort.Stop();
				TimerWaitLong.Stop();
				TimerRollText.Stop();
				TimerSound.Stop();
				allowAdvance = true;
				buffer = true;
				TimerBuffer.Start();
			}
		}

		Update();
	}


	public override void _Draw()
	{
		if (disp >= 0)
		{
			// Draw nametag
			DrawString(font, new Vector2(textLeft, NametagTop), talkSide ? rightClientName : leftClientName, talkSide ? new Color(rightClientColor.r, rightClientColor.g, rightClientColor.b, textAlpha) : new Color(leftClientColor.r, leftClientColor.g, leftClientColor.b, textAlpha));
			
			// Draw dialogue text
			Modifier modifier = Modifier.NORMAL;

			int i = 0;
			int line = 0;
			float charSpacing = 0f;

			while (i < disp + 1)
			{
				// Line breaks
				if (i > 0 && text[textPage][i - 1] == ' ' && charSpacing + wordRegex.Match(text[textPage], i).ToString().Length > lineEnd)
				{
					charSpacing = 0f;
					line++;
				}
				
				// Escape tokens
				switch (text[textPage][i])
				{
					case '#':
					{
						charSpacing = 0f;
						line++;
						break;
					}

					case '|':
					{
						if (!pause)
						{
							TimerRollText.Stop();
							TimerSound.Stop();
							pause = true;
							roll = false;
							text[textPage] = text[textPage].Remove(i, 1);
							charSpacing--;
							TimerWaitShort.Start();
						}

						break;
					}

					case '{':
					{
						if (!pause)
						{
							TimerRollText.Stop();
							TimerSound.Stop();
							pause = true;
							roll = false;
							text[textPage] = text[textPage].Remove(i, 1);
							charSpacing--;
							TimerWaitLong.Start();
						}

						break;
					}

					case '$':
					{
						switch (text[textPage][i + 1])
						{
							case '0':
								modifier = Modifier.NORMAL;
								break;
							case 'r':
								modifier = Modifier.RED;
								break;
							case 'b':
								modifier = Modifier.BLUE;
								break;
							case 'g':
								modifier = Modifier.GREEN;
								break;
							case 'y':
								modifier = Modifier.YELLOW;
								break;
							case 's':
								modifier = Modifier.SHAKE;
								break;
							case 'w':
								modifier = Modifier.WAVE;
								break;
							default:
								modifier = Modifier.NORMAL;
								break;
						}

						i++;
						break;
					}

					default:
					{
						font.Size = textSize;
						switch (modifier)
						{
							case Modifier.NORMAL:
								charSpacing += DrawChar(font, new Vector2(textLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), string.Empty, new Color(1f, 1f, 1f, textAlpha));
								break;
							case Modifier.RED:
								charSpacing += DrawChar(font, new Vector2(textLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), string.Empty, new Color(1f, 0, 0, textAlpha));
								break;
							case Modifier.BLUE:
								charSpacing += DrawChar(font, new Vector2(textLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), string.Empty, new Color(0, 1f, 1f, textAlpha));
								break;
							case Modifier.GREEN:
								charSpacing += DrawChar(font, new Vector2(textLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), string.Empty, new Color(0, 0.8f, 0, textAlpha));
								break;
							case Modifier.YELLOW:
								charSpacing += DrawChar(font, new Vector2(textLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), string.Empty, new Color(0.9f, 0.57f, 0.11f, textAlpha));
								break;
							case Modifier.SHAKE:
								charSpacing += DrawChar(font, new Vector2(textLeft + charSpacing + (float)GD.RandRange(-1d, 1d), TextTop + (LineSpacing * line) + Mathf.RoundToInt((float)GD.RandRange(-1d, 1d))), text[textPage][i].ToString(), string.Empty, new Color(1f, 1f, 1f, textAlpha));
								break;
							case Modifier.WAVE:
							{
								float s = (2f * t) + (i * 3);
								double shift = Math.Sin(s * Math.PI * (1f / 60f)) * 3f;
								charSpacing += DrawChar(font, new Vector2(textLeft + charSpacing, TextTop + (LineSpacing * line) + (float)shift), text[textPage][i].ToString(), string.Empty, new Color(1f, 1f, 1f, textAlpha));
								break;
							}

							default:
								charSpacing += DrawChar(font, new Vector2(textLeft + charSpacing, TextTop + (LineSpacing * line)), text[textPage][i].ToString(), string.Empty, new Color(1f, 1f, 1f, textAlpha));
								break;
						}

						break;
					}
				}

				i++;
			}
		}
	}

	// ================================================================

	public void InitializePortraits()
	{
		PortraitFrameLeft.SelfModulate = leftClientColor;
		PortraitFrameRight.SelfModulate = rightClientColor;
	}

	// ================================================================

	private void Start()
	{
		// Load text
		if (textOverride == "")
			LoadTextFromFile(sourceFile);
		else
		{
			foreach (string s in textOverride.Split("\n"))
			{
				var currentLine = lineRegex.Match(s);
				clients.Add(currentLine.Groups[1].ToString() == "1");
				clientExpressionsLeft.Add(currentLine.Groups[2].ToString());
				clientExpressionsRight.Add(currentLine.Groups[3].ToString());
				text.Add(currentLine.Groups[4].ToString());
			}
		}

		textPage = 0;
		disp = 0;

		/*// Set up portraits
		PortraitSpriteLeft.Frames = leftClientPortrait;
		
		if (secondClient)
			PortraitSpriteRight.Frames = rightClientPortrait;
		else
		{
			PortraitFrameRight.Hide();
			PortraitSpriteRight.Hide();
		}*/

		UpdatePortraits();

		font.Size = textSize;
		TimerRollText.SetWaitTime(interval);
		TimerRollText.Start();
		PlayTextSound();
		TimerSound.Start();
		started = true;
	}


	private void LoadTextFromFile(string path)
	{
		File file = new File();

		try
		{
			file.Open(path, (int)File.ModeFlags.Read);
			int currentIndex = -1;
			bool read = false;

			while (!file.EofReached())
			{
				string line = file.GetLine();
				if (line.Length > 0 && line[0] == '[')
					//currentIndex = line[1].ToString().ToInt();
					currentIndex = numberRegex.Match(line).Groups[1].ToString().ToInt();
				
				if (line.Length > 0 && line[0] == '}' && read)
				{
					read = false;
					break;
				}

				if (read)
				{
					var currentLine = lineRegex.Match(line);
					clients.Add(currentLine.Groups[1].ToString() == "1");
					clientExpressionsLeft.Add(currentLine.Groups[2].ToString());
					clientExpressionsRight.Add(currentLine.Groups[3].ToString());
					text.Add(currentLine.Groups[4].ToString());
				}
				
				if (line.Length > 0 && line[0] == '{' && currentIndex == textSet)
					read = true;
			}
		}
		finally
		{
			if (file.IsOpen())
				file.Close();
		}
	}


	private void RollText()
	{
		disp++;
		if (disp >= text[textPage].Length - 1)
		{
			roll = false;
			// indicator
			TimerRollText.Stop();
			TimerSound.Stop();
			allowAdvance = true;
		}
	}


	private void PlayTextSound()
	{
		Controller.PlaySoundBurst(SoundType.Stream, volume: -2, pitch: (float)GD.RandRange(0.96, 1.04));
	}


	private void AdvancePage()
	{
		if (textPage + 1 < text.Count)
		{
			SoundAdvance.Play();
			Indicator.Hide();
			textPage++;
			disp = -1;
			UpdatePortraits();

			TimerRollText.SetWaitTime(interval);
			TimerRollText.Start();
			PlayTextSound();
			TimerSound.Start();
			allowAdvance = false;
			roll = true;
		}
		else
		{
			Controller.PlaySoundBurst(SoundEnd.Stream, volume: 4f);
			Indicator.Hide();
			AnimPlayer.Play("Finish");
			finished = true;
			TimerEnd.Start();
		}
	}


	private void UpdatePortraits()
	{
		talkSide = clients[textPage];
		PortraitSpriteLeft.Play(clientExpressionsLeft[textPage]);
		PortraitSpriteRight.Play(clientExpressionsRight[textPage]);
	}


	private void ResumeText()
	{
		TimerRollText.Start();
		PlayTextSound();
		TimerSound.Start();
		pause = false;
		roll = true;
	}


	private void ResetBuffer()
	{
		buffer = false;
	}


	private void Finish()
	{
		if (restoreMovement)
			Player.State = Player.ST.MOVE;

		EmitSignal("text_ended");
		QueueFree();
	}
}
