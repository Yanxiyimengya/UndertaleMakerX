using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[GlobalClass]
[Tool]
public partial class TextTyper : Control
{
	[Export(PropertyHint.MultilineText)]
	public string Text = "";

	[ExportGroup("Typer")]
	[Export]
	public bool Typing
	{
		get => _typing;
		set
		{
			if (_typing != value)
			{
				_typing = value;
				if (value && _End() && IsInstanceValid(this) && IsInsideTree())
				{
					Start(Text);
				}
			}
		}
	}
	[Export]
	public bool AutoStart = false;
	[Export]
	public bool Instant = false;
	[Export]
	public double TyperSpeed = 0.1;
	[Export]
	public AudioStream Voice = null;

	[ExportGroup("Text")]
	[Export]
	public Godot.Font TyperFont
	{
		get => _typerFont;
		set
		{
			_typerFont = value;
		}
	}
	[Export]
	public Color FontColor = Colors.White;
	[Export]
	public int FontSize = 16;
	[Export]
	public int LineSpace = 3;
	[Export]
	public bool Autowarp = false;

	private int _typerProgress = 0;
	private bool _typing = false;
	private bool _finished = false;
	private double _typerProcessTimer = 0.0;
	private double _typerWaitTimer = 0.0;
	private Vector2 _typerCursorPosition = new Vector2();
	private Godot.Font _typerFont = ThemeDB.FallbackFont;
	private List<Rid> _canvasItemList = new List<Rid>();
	private Queue<Rid> _canvasItemPoolList = new Queue<Rid>();

	public override void _Ready()
	{
		if (!Engine.IsEditorHint()) { 
			if (AutoStart && IsInstanceValid(this) && IsInsideTree()) { 
				Start(Text);
			}
		}
	}

	public override void _Draw()
	{
		if (Engine.IsEditorHint())
		{
			while (!_End())
			{
				_ProcessChar();
			}
		}
		else
		{
			if (_finished || _typing)
			{
				int processed = _typerProgress;
				ResetData();
				_finished = false;
				_typing = true;
				for (int i = 0; i < processed; i ++)
				{
					_ProcessChar();
				}
			}
		}
	}

	public override void _ExitTree()
	{
		foreach (Rid id in _canvasItemList)
		{
			if (id.IsValid)
				RenderingServer.FreeRid(id);
		}
		_canvasItemList.Clear();
		foreach (Rid id in _canvasItemPoolList)
		{
			if (id.IsValid)
				RenderingServer.FreeRid(id);
		}
		_canvasItemPoolList.Clear();
	}

	public override void _Process(double delta)
	{
		if (!_typing || !IsInstanceValid(this) || !IsInsideTree())
			return;

		if (_End())
		{
			_typing = false;
			_finished = true;
			return;
		}

		if (_typerWaitTimer > 0.0)
		{
			_typerWaitTimer -= delta;
		}
		else
		{
			if (_typerProcessTimer > 0.0)
			{
				_typerProcessTimer -= delta;
			}
			else
			{
				_typerProcessTimer = TyperSpeed;
				_ProcessChar();
			}
		}
	}

	private void _ProcessChar()
	{
		while (!_End())
		{
			char c = Text[_typerProgress];
			_typerProgress += 1;
			while (c == '[')
			{
				int locate = Text.FindN("]", _typerProgress + 1);
				if (locate != -1)
				{
					string content = Text.Substring(_typerProgress, locate - _typerProgress);
					string[] contentElements = content.Split(' ');
					_typerProgress = locate + 1;
					_ProcessCmd(contentElements);
				}
				if (_End()) return;
				c = Text[_typerProgress];
				_typerProgress += 1;
			}
			
			if (c == ' ' || c == '\t')
			{
				_Step(c);
				continue;
			}
			else if (c == '\r' || c == '\n')
			{
				_NewLine();
				continue;
			}
			else
			{
				_PrintChar(c);
				if (Voice != null && !Instant && !Engine.IsEditorHint() && IsInstanceValid(GlobalStreamPlayer.Instance))
				{
					GlobalStreamPlayer.Instance.PlaySound(Voice);
				}
				if (!Instant) break;
			}
		}
	}

	private void _PrintChar(long @char)
	{
		if (!IsInstanceValid(this) || !IsInsideTree() || _typerFont == null)
			return;

		double _typerFontHeight = _typerFont.GetHeight(FontSize);
		_GetCharAttribute(@char, out Vector2 charAdvance);

		if (Autowarp && GetRect().Size.X > 0 && (_typerCursorPosition.X + charAdvance.X) >= GetRect().Size.X)
			_NewLine();

		double asc = _typerFont.GetAscent(FontSize);
		if (asc > _typerFontHeight)
		{
			double diff = _typerFontHeight - asc;
			asc += diff / 2;
		}

		Transform2D trans = Transform2D.Identity.Translated(_typerCursorPosition + new Vector2(0.0F, (float)asc));

		Rid canvasItemRid = new Rid();
		bool requireCreateNew = true;

		while (_canvasItemPoolList.Count > 0)
		{
			if (!_canvasItemPoolList.TryDequeue(out canvasItemRid))
				break;
			if (canvasItemRid.IsValid)
			{
				RenderingServer.CanvasItemSetParent(canvasItemRid, GetCanvasItem());
				RenderingServer.CanvasItemSetVisible(canvasItemRid, true);
				requireCreateNew = false;
				break;
			}
		}

		if (requireCreateNew)
		{
			Rid parentCanvasItem = GetCanvasItem();
			if (parentCanvasItem.IsValid)
			{
				canvasItemRid = RenderingServer.CanvasItemCreate();
				RenderingServer.CanvasItemSetParent(canvasItemRid, parentCanvasItem);
			}
		}

		if (canvasItemRid.IsValid)
		{
			_canvasItemList.Add(canvasItemRid);
			RenderingServer.CanvasItemClear(canvasItemRid);
			RenderingServer.CanvasItemSetTransform(canvasItemRid, trans);
			_typerFont.DrawChar(canvasItemRid, Vector2.Zero, @char, FontSize, FontColor);
		}

		_Step(@char);
	}

	private void _NewLine()
	{
		_typerCursorPosition = new Vector2(0.0F,
			_typerCursorPosition.Y + (float)_typerFont.GetHeight(FontSize) + LineSpace);
	}

	private void _Step(long @char)
	{
		if (_typerFont == null)
			return;

		_GetCharAttribute(@char, out Vector2 charAdvance);
		_typerCursorPosition = _typerCursorPosition + new Vector2(charAdvance.X, 0.0F);
		CustomMinimumSize = _typerCursorPosition + new Vector2(charAdvance.X, (float)_typerFont.GetHeight(FontSize));
	}

	private void _GetCharAttribute(long @char, out Vector2 glyphAdvance)
	{
		glyphAdvance = Vector2.Zero;
		if (_typerFont == null)
			return;

		if (_typerFont is FontVariation font)
		{
			glyphAdvance += new Vector2(font.SpacingGlyph, 0F);
			if (@char == ' ')
			{
				glyphAdvance += new Vector2(font.SpacingSpace, 0F);
			}
		}

		foreach (Rid rid in _typerFont.GetRids())
		{
			TextServer ts = TextServerManager.GetPrimaryInterface();
			if (ts != null && ts.FontHasChar(rid, @char))
			{
				long glyphIndex = ts.FontGetGlyphIndex(rid, FontSize, @char, 0);
				glyphAdvance += ts.FontGetGlyphAdvance(rid, FontSize, glyphIndex);
				return;
			}
		}
	}

	private bool _End()
	{
		return _typerProgress >= Text.Length;
	}

	private void _ProcessCmd(string[] cmd)
	{
		if (cmd == null || cmd.Length == 0)
			return;

		switch (cmd[0])
		{
			case "wait":
				if (cmd.Length >= 2 && float.TryParse(cmd[1], out float waitTime))
				{
					_typerWaitTimer = waitTime;
				}
				break;

			case "color":
				if (cmd.Length >= 2)
				{
					FontColor = Color.FromString(cmd[1], FontColor);
				}
				break;

			case "instant":
				if (cmd.Length > 1 && bool.TryParse(cmd[1], out bool _cmd1))
				{
					Instant = _cmd1;
				}
				else
				{
					Instant = !Instant;
				}
				break;

			case "font":
				if (cmd.Length > 1)
				{
					Font fnt = (Font)UTMXResourceLoader.Load(cmd[1]);
					if (fnt != null)
					{
						TyperFont = fnt;
					}
				}
				break;

			default:
				break;
		}
	}

	public void Start(string text)
	{
		if (!IsInstanceValid(this) || !IsInsideTree() || string.IsNullOrEmpty(text))
		{
			_typing = false;
			return;
		}
		foreach (Rid id in _canvasItemList)
		{
			if (id.IsValid)
			{
				RenderingServer.CanvasItemSetVisible(id, false);
				_canvasItemPoolList.Enqueue(id);
			}
		}
		_canvasItemList.Clear();

		Text = text;
		ResetData();

		_typing = true;
		_finished = false;
	}

	public void ResetData()
	{
		_typerProgress = 0;
		_typerProcessTimer = 0.0;
		_typerWaitTimer = 0.0;
		_typerCursorPosition = Vector2.Zero;
		CustomMinimumSize = Vector2.Zero;
		FontColor = Colors.White;
	}
}
