using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Godot.TextServer;
using static System.Runtime.InteropServices.JavaScript.JSType;

[GlobalClass]
public partial class TextTyper : Godot.RichTextLabel, IObjectPoolObject
{
	[Export(PropertyHint.MultilineText)]
	public string TyperText = "";
	[Export]
	public double TyperSpeed = 0.05;
	[Export]
	public int TyperSize
	{
		get => _typerSize;
		set
		{
			_typerSize = value;
			PushFontSize(value);
		}
	}
	[Export]
	public Color TyperColor
	{
		get => _typerColor;
		set
		{
			_typerColor = value;
			PushColor(value);
		}
	}
	[Export]
	public Font TyperFont
	{
		get => _typerFont;
		set
		{
			_typerFont = value;
			if (value != null)
				PushFont(value, TyperSize);
		}
	}
	[Export]
	public bool Instant = false;
	[Export]
	public bool NoSkip = false;

	[Export]
	public AudioStream Voice = null;
    public GameShader ShaderInstance
    {
        get => _shaderInstance;
        set
        {
            _shaderInstance = value;
            Material = value.GetShaderMaterial();
        }
    }

    private double _typerTimer = 0.0;
	private int _typerSize = 16;
	private double _typerWattingTimer = 0.0;
	private int _typerProgress = 0;
	private Font _typerFont = ThemeDB.FallbackFont;
	private Color _typerColor = Colors.White;
	private object _waitForKeyAction = null;
    protected GameShader _shaderInstance;

	public TextTyper()
	{
		BbcodeEnabled = true;
		ScrollActive = false;
		FitContent = true;
		AutowrapMode = AutowrapMode.Off;
	}

	public override void _Process(double delta)
	{
		if (!_CanRunning())
		{
			if (_waitForKeyAction != null)
			{
				if (_waitForKeyAction is string actString && Input.IsActionPressed(actString) ||
					_waitForKeyAction is Key actKey && Input.IsKeyPressed(actKey))
					_waitForKeyAction = null;
			}
		}
		else
		{
			if (_typerWattingTimer > 0.0) _typerWattingTimer -= delta;
			else
			{
				if (_typerTimer > 0.0) _typerTimer -= delta;
				else
				{
					_typerTimer = TyperSpeed;
					_ProcessText();
				}

				if (!NoSkip && !Instant && !Engine.IsEditorHint())
				{
					if (Input.IsActionJustPressed("cancel"))
					{
						Instant = true;
						_typerTimer = 0.0;
					}
				}
			}
		}
	}

	private void _ProcessText()
	{
		while (_CanRunning())
		{
			char c = TyperText[_typerProgress];
			_typerProgress += 1;
			while (c == '[')
			{
				int locate = TyperText.FindN("]", _typerProgress);
				if (locate != -1)
				{
					string content = TyperText.Substring(_typerProgress, locate - _typerProgress);
					if (!string.IsNullOrEmpty(content))
					{
						if (_ParseBBCodeTag(content, out string cmdName, out Dictionary<string, string> directParameters))
						{
							try
							{
								if (!_ProcessCmd(cmdName, directParameters))
								{
									AppendText($"[{content}]");
								}
							}
							catch (Exception e) {
								UtmxLogger.Error(e.Message);
							}
						}
					}
					_typerProgress = locate + 1;
				}
				if (!_CanRunning())
				{
					return;
				}
				else
				{
					c = TyperText[_typerProgress];
					_typerProgress += 1;
				}
			}

			while (c == '\r' || c == '\n')
			{
				_typerProgress += 1;
				c = TyperText[_typerProgress];
				Newline();
			}

			if (_typerWattingTimer <= 0.0)
			{
				AddText(c.ToString());
			}
			if (!Instant)
			{
				if (Voice != null && !Engine.IsEditorHint())
				{
					UtmxGlobalStreamPlayer.PlaySoundFromStream(Voice);
				}
				break;
			}
		}
	}
	private bool _ParseBBCodeTag(string content, out string cmdName, out System.Collections.Generic.Dictionary<string, string> directParameters)
	{
		cmdName = string.Empty;
		directParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		var tokens = new System.Collections.Generic.List<string>();
		bool inQuotes = false;
		StringBuilder currentToken = new StringBuilder();

		for (int i = 0; i < content.Length; i++)
		{
			char c = content[i];
			if (c == '\"' || c == '\'')
			{
				inQuotes = !inQuotes;
				continue;
			}
			if ((c == ' ' || c == ',') && !inQuotes)
			{
				if (currentToken.Length > 0)
				{
					tokens.Add(currentToken.ToString());
					currentToken.Clear();
				}
				continue;
			}
			currentToken.Append(c);
		}

		if (currentToken.Length > 0)
		{
			tokens.Add(currentToken.ToString());
		}
		if (tokens.Count == 0) return false;

		string firstToken = tokens[0];
		if (firstToken.Contains('='))
		{
			var parts = SplitToken(firstToken);
			if (parts.Length == 2)
			{
				string paramName = parts[0].Trim();
				string paramValue = parts[1].Trim();
				cmdName = paramName;
				directParameters["value"] = paramValue;
				return true;
			}
		}
		else
		{
			cmdName = firstToken;
			for (int i = 1; i < tokens.Count; i++)
			{
				string token = tokens[i];
				if (token.Contains('='))
				{
					var parts = SplitToken(token);
					if (parts.Length >= 2)
					{
						string paramName = parts[0].Trim();
						string paramValue = string.Join("=", parts.Skip(1)).Trim();

						directParameters[paramName] = paramValue;
					}
				}
			}
			return true;
		}
		return false;
	}

	private string[] SplitToken(string token)
	{
		int equalsIndex = token.IndexOf('=');
		if (equalsIndex > 0)
		{
			return new string[]
			{
				token.Substring(0, equalsIndex),
				token.Substring(equalsIndex + 1)
			};
		}
		return new string[] { token };
	}

	public virtual bool _ProcessCmd(string cmd, Dictionary<string, string> args)
	{
		switch (cmd)
		{
			case "waitfor":
			{
				if (args.TryGetValue("value", out string keyValue))
				{
					if (string.IsNullOrEmpty(keyValue)) break;
					if (InputMap.HasAction(keyValue))
					{
						_waitForKeyAction = keyValue;
					}
					else
					{
						Key key = OS.FindKeycodeFromString(keyValue);
						if (key != Key.None)
						{
							_waitForKeyAction = key;
						}
					}
				}
				break;
			}

			case "wait":
			{
				if (args.TryGetValue("value", out string waitTime) && float.TryParse(waitTime, out float time))
				{
					_typerWattingTimer = time;
				}
				break;
			}

			case "blend":
			{
				if (args.TryGetValue("value", out string blendColor))
				{
					Modulate = Color.FromString(blendColor, Modulate);
				}
				break;
			}

			case "speed":
			{
				if (args.TryGetValue("value", out string spdValue) && float.TryParse(spdValue, out float spd))
				{
					TyperSpeed = (float)spd;
				}
				break;
			}

			case "size":
			{
				if (args.TryGetValue("value", out string sizeValue) && float.TryParse(sizeValue, out float fntSize))
				{
					TyperSize = (int)fntSize;
				}
				break;
			}

			case "font":
			{
				if (args.TryGetValue("value", out string fontPath) || args.Count > 0)
				{
					Font fnt = UtmxResourceLoader.Load(fontPath) as Font;
					if (fnt != null)
					{
						PushFont(fnt);
					}
				}
				break;
			}

			case "instant":
			{
				if (args.TryGetValue("value", out string instantValue) && bool.TryParse(instantValue, out bool _ins))
				{
					Instant = _ins;
				}
				else
				{
					Instant = !Instant;
				}
				break;
			}

			case "noskip":
				{
					if (args.TryGetValue("value", out string noskipValue) && bool.TryParse(noskipValue, out bool _noskip))
					{
						NoSkip = _noskip;
					}
					else
					{
						NoSkip = !NoSkip;
					}
					break;
				}

			case "clear":
			{
				Clear();
				TyperColor = TyperColor;
				TyperFont = TyperFont;
				TyperSize = TyperSize;
				Instant = false;
				break;
			}
			case "end":
				{
					Destroy();
					break;
				}

			case "img":
			{
				if (args.TryGetValue("path", out string imgPath) && !string.IsNullOrEmpty(imgPath))
				{
					Texture2D texture = UtmxResourceLoader.Load(imgPath) as Texture2D;
					if (texture != null)
					{
						int width = 0, height = 0;
						Color col = Colors.White;
						if (args.TryGetValue("width", out string argWidth))
						{
							int.TryParse(argWidth, out width);
						}
						if (args.TryGetValue("height", out string argHeight))
						{
							int.TryParse(argHeight, out height);
						}
						if (args.TryGetValue("color", out string argColor))
						{
							col = Color.FromString(argColor, col);
						}
						AddImage(texture, width, height, col);
					}
				}
				break;
			}


			// INLINE COMMANDS
			case "voice":
				if (Instant) return true;
				if (args.TryGetValue("value", out string voicePath) && !string.IsNullOrEmpty(voicePath))
				{
					if (voicePath == "null")
					{
						Voice = null;
						break;
					}
					AudioStream voiceStream = UtmxResourceLoader.Load(voicePath) as AudioStream;
					if (voiceStream != null)
					{
						Voice = voiceStream;
					}
				}
				break;

			case "play_sound":
			{
				if (Instant) return true;
				if (args.TryGetValue("value", out string soundPath) && !string.IsNullOrEmpty(soundPath))
				{
					AudioStream soundStream = UtmxResourceLoader.Load(soundPath) as AudioStream;
					if (soundStream != null)
					{
						UtmxGlobalStreamPlayer.PlaySoundFromStream(soundStream);
					}
				}
				break;
			}

			case "play_bgm":
			{
				if (Instant) return true;
				if (args.TryGetValue("path", out string bgmPath) && !string.IsNullOrEmpty(bgmPath))
				{
					args.TryGetValue("id", out string bgmId);
					if (string.IsNullOrEmpty(bgmId)) bgmId = "_TYPER_BGM";

					AudioStream bgmStream = UtmxResourceLoader.Load(bgmPath) as AudioStream;
					if (bgmStream == null) break;

					bool loop = false;
					if (args.TryGetValue("loop", out string bgmLoopStr)) bool.TryParse(bgmLoopStr, out loop);

					UtmxGlobalStreamPlayer.PlayBgmFromStream(bgmId, bgmStream, loop);

					if (args.TryGetValue("pitch", out string bgmPitchStr) && float.TryParse(bgmPitchStr, out float bgmPitch))
						UtmxGlobalStreamPlayer.SetBgmPitch(bgmId, bgmPitch);
					if (args.TryGetValue("volume", out string bgmVolumeStr) && float.TryParse(bgmPitchStr, out float bgmVolume))
						UtmxGlobalStreamPlayer.SetBgmVolume(bgmId, bgmVolume);
				}
				break;
			}

			case "stop_bgm":
			{
				if (Instant) return true;
				string bgmId;
				if (args.TryGetValue("value", out bgmId) && string.IsNullOrEmpty(bgmId))
				{
					UtmxGlobalStreamPlayer.StopBgm(bgmId);
				}
				else if (args.TryGetValue("id", out bgmId) && string.IsNullOrEmpty(bgmId))
				{
					UtmxGlobalStreamPlayer.StopBgm(bgmId);
				}
				else
				{
					UtmxGlobalStreamPlayer.StopBgm("_TYPER_BGM");
				}
				break;
			}
			default:
				return false;
		}
		return true;
	}

	private bool _CanRunning()
	{
		return (_waitForKeyAction == null) && (!IsFinished());
	}
	public void Start(string text = null)
	{
		Clear();
		ResetData();
		if (text == null) return;
		TyperText = text;
	}

	public new bool IsFinished()
	{
		return _typerProgress >= TyperText.Length;
    }
    public int GetProgress()
    {
        return _typerProgress;
    }

    public void ResetData()
	{
		_typerProgress = 0;
		_typerTimer = 0.0;
		_typerWattingTimer = 0.0;
		TyperFont = TyperFont;
		TyperSize = TyperSize;
		TyperColor = TyperColor;
		NoSkip = false;
		Instant = false;
		Modulate = Colors.White;
		TyperText = "";
	}

	public virtual void Awake()
	{
		Position = Vector2.Zero;
		Scale = Vector2.One;
		Start(null);
	}

	public virtual void Disabled()
	{
	}


	public virtual void Destroy()
	{
		UtmxSceneManager.DeleteTextTyper(this);
	}
}
