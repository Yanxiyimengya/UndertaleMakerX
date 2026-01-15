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
	[Export]
	public double TyperSpeed = 0.1;
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
	public Color FontColor = Color.Color8(255, 255, 255);
	[Export]
	public int FontSize = 16;
	[Export]
	public int LineSpace = 3;
	[Export]
	public AudioStream Voice = null;

	[Export]
	public bool Typing
	{
		get => _typing;
		set
		{
			if (Engine.IsEditorHint() && value && !_typing)
			{
				Restart(Text);
			}
			_typing = value;
		}
	}
	[Export]
	public bool AutoStart = false;

	private int _typerProgress = 0;
	private bool _typing = false;
	private double _typerProcessTimer = 0.0;
	private double _typerWaitTimer = 0.0;
	private Vector2 _typerCursorPosition = new Vector2();
	private Godot.Font _typerFont = ThemeDB.FallbackFont;
	private List<Rid> _canvasItemList = new List<Rid>();
	private Queue<Rid> _canvasItemPoolList = new Queue<Rid>();

	public override void _Ready()
	{
		if (AutoStart) Typing = true;
	}
	public override void _ExitTree()
	{
		foreach (Rid id in _canvasItemList)
		{
			RenderingServer.FreeRid(id);
		}
		foreach (Rid id in _canvasItemPoolList)
		{
			RenderingServer.FreeRid(id);
		}
	}

	public override void _Process(double delta)
	{
		if (!Typing) return;
		if (!_End())
		{
			if (_typerWaitTimer > 0.0) { 
				_typerWaitTimer -= delta;
				return;
			}

			if (_typerProcessTimer > 0.0)
			{
				_typerProcessTimer -= delta;

			}
			else
			{
				_typerProcessTimer = TyperSpeed;
				char c;
				do
				{
					c = Text[_typerProgress];
					_typerProgress += 1;

					if (c == '[')
					{
						int locate = Text.FindN("]", _typerProgress);
						if (locate != -1)
						{
							string content = Text.Substring(_typerProgress, locate - _typerProgress);
							string[] contentElements = content.Split(' ');
							_typerProgress = locate + 1;
							_ProcessCmd(contentElements);
						}
						continue;
					}

					if (c == ' ' || c == '	')
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
						if (Voice != null && !Engine.IsEditorHint())
						{
							GlobalSoundPlayer.Instance.PlaySound(Voice);
						}
						break;
					}
				}
				while (!_End());
			}
		}
		else
		{
			Typing = false;
		}
	}
	
	private void _PrintChar(long @char)
	{
		double _typerFontHeight = _typerFont.GetHeight(FontSize);
		_GetCharAttribute(@char, out Vector2 charAdvance);
		if ((_typerCursorPosition.X + charAdvance.X) >= GetRect().Size.X) _NewLine();

		double asc = TyperFont.GetAscent(FontSize);
		if (asc > _typerFontHeight)
		{
			double diff = _typerFontHeight - asc;
			asc += diff / 2;
		}
		Transform2D trans = Transform2D.Identity.Translated(_typerCursorPosition + new Vector2(0.0F, (float)asc));
		
		Rid canvasItemRid;
		if (_canvasItemPoolList.Count > 0)
		{
			canvasItemRid = _canvasItemPoolList.Dequeue();
			RenderingServer.CanvasItemSetVisible(canvasItemRid, true);
		}
		else
		{
			canvasItemRid = RenderingServer.CanvasItemCreate();
			RenderingServer.CanvasItemSetParent(canvasItemRid, GetCanvasItem());
		}
		_canvasItemList.Add(canvasItemRid);
		RenderingServer.CanvasItemClear(canvasItemRid);
		RenderingServer.CanvasItemSetTransform(canvasItemRid, trans);
		TyperFont.DrawChar(canvasItemRid, Vector2.Zero, @char, FontSize, FontColor);
		_Step(@char);
	}

	private void _NewLine()
	{
		_typerCursorPosition = new Vector2(0.0F, 
			_typerCursorPosition.Y + (float)_typerFont.GetHeight(FontSize) + LineSpace);
	}

	private void _Step(long @char)
	{
		_GetCharAttribute(@char, out Vector2 charAdvance);
		if ((_typerCursorPosition.X + charAdvance.X) >= GetRect().Size.X) _NewLine();
		_typerCursorPosition = _typerCursorPosition + new Vector2(charAdvance.X, 0.0F);
	}

	private void _GetCharAttribute(long @char, out Vector2 glyphAdvance)
	{
		glyphAdvance = Vector2.Zero;
		if (TyperFont is FontVariation font)
		{
			glyphAdvance += new Vector2(0F, font.SpacingGlyph);
			if (@char == ' ')
			{
				glyphAdvance += new Vector2(0F, font.SpacingSpace);
			}
		}
		foreach (Rid rid in TyperFont.GetRids())
		{
			TextServer ts = TextServerManager.GetPrimaryInterface();
			if (ts.FontHasChar(rid, @char))
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
		switch(cmd[0])
		{
			case "wait":
				float wintTime = cmd[1].ToFloat();
				_typerWaitTimer = wintTime;
				break;

			case "color":
				FontColor = Color.FromString(cmd[1], FontColor);
				break;
		}
	}




	public void Restart(string text)
	{
		foreach (Rid id in _canvasItemList)
		{
			_canvasItemPoolList.Enqueue(id);
			RenderingServer.CanvasItemSetVisible(id, false);
		}
		_canvasItemList.Clear();

		Text = text;
		_typerProgress = 0;
		_typerProcessTimer = 0.0;
		_typerWaitTimer = 0.0;
		_typerCursorPosition = Vector2.Zero;
		Typing = true;
	}

}
