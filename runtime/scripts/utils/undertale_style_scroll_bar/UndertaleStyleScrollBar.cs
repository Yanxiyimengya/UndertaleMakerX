using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class UndertaleStyleScrollBar : Control
{
	[Export]
	public int Count
	{
		get => _count;
		set
		{
			_count = Math.Max(0, value);
			QueueRedraw();
		}
	}
	[Export]
	public int CurrentIndex
	{
		get => _currentIndex;
		set
		{
			_currentIndex = Math.Clamp(value, 0, Math.Max(0, Count - 1));
			QueueRedraw();
		}
	}
	[Export]
	public Vector2 PointSize
	{
		get => _pointSize;
		set
		{
			_pointSize = value;
			QueueRedraw();
		}
	}
	[Export]
	public Vector2 PointFocusSize
	{
		get => _pointFocusSize;
		set
		{
			_pointFocusSize = value;
			QueueRedraw();
		}
	}
	[Export]
	public float Spacing
	{
		get => _spacing;
		set
		{
			_spacing = value;
			QueueRedraw();
		}
	}

	[Export]
	public float PageSize = 3;

	[Export]
	public Texture2D ArrowTexture;

	private int _count = 0;
	private int _currentIndex = 0;
	private Vector2 _pointSize = new Vector2(4, 4);
	public Vector2 _pointFocusSize = new Vector2(10, 10);
	private float _spacing = 6;
	private bool _inTop = false;
	private bool _inBottom = false;
	private Rid _topArrow;
	private Rid _bottomArrow;
	private double _arrowAnimTimer = 0.0;
	private double _arrowAnimOffset = 0.0;
	private Tween _tween;

	public override void _EnterTree()
	{
		_topArrow = RenderingServer.CanvasItemCreate();
		RenderingServer.CanvasItemSetParent(_topArrow, GetCanvasItem());
		_bottomArrow = RenderingServer.CanvasItemCreate();
		RenderingServer.CanvasItemSetParent(_bottomArrow, GetCanvasItem());

		if (ArrowTexture != null)
		{
			RenderingServer.CanvasItemAddTextureRect(_topArrow, new Rect2(-ArrowTexture.GetSize() / 2, ArrowTexture.GetSize()), ArrowTexture.GetRid());
			RenderingServer.CanvasItemAddTextureRect(_bottomArrow, new Rect2(-ArrowTexture.GetSize() / 2, ArrowTexture.GetSize()), ArrowTexture.GetRid());
		}
	}

	public override void _ExitTree()
	{
		if (_topArrow.IsValid)
			RenderingServer.FreeRid(_topArrow);
		if (_bottomArrow.IsValid)
			RenderingServer.FreeRid(_bottomArrow);
	}

	public override void _Process(double delta)
	{
		_inTop = _inTop ? (_currentIndex < PageSize) : (_currentIndex == 0);
		_inBottom = _inBottom ? (_currentIndex >= Count - PageSize) : (_currentIndex == Count - 1);

		if (Count > PageSize && ArrowTexture != null)
		{
			float halfCount = Count / 2.0f;
			float totalPointHeight = _pointSize.Y * Count;
			float totalSpacing = Spacing * (Count - 1);
			float centerOffset = (totalPointHeight + totalSpacing) / 2;

			float topY = -centerOffset + _pointSize.Y / 2 + Spacing / 2 - (float)_arrowAnimOffset;
			RenderingServer.CanvasItemSetVisible(_topArrow, !_inTop);
			RenderingServer.CanvasItemSetTransform(_topArrow,
				Transform2D.Identity.Translated(new Vector2(0F, topY - 10)));

			float bottomY = centerOffset - _pointSize.Y / 2 - Spacing / 2 + (float)_arrowAnimOffset;
			RenderingServer.CanvasItemSetVisible(_bottomArrow, !_inBottom);
			RenderingServer.CanvasItemSetTransform(_bottomArrow,
				Transform2D.Identity.Scaled(new Vector2(1F, -1F)).Translated(new Vector2(0F, bottomY + 10)));

			if (_arrowAnimTimer > 0)
			{
				_arrowAnimTimer -= delta;
			}
			else
			{
				_arrowAnimTimer = 1.0F;
				if (_tween != null && _tween.IsRunning())
				{
					_tween.Kill();
				}
				_tween = GetTree().CreateTween();
				_tween.TweenProperty(this, "_arrowAnimOffset", 5, 0.42).From(0.0);
			}
		}
		else
		{
			if (_topArrow.IsValid)
				RenderingServer.CanvasItemSetVisible(_topArrow, false);
			if (_bottomArrow.IsValid)
				RenderingServer.CanvasItemSetVisible(_bottomArrow, false);
		}
	}

	public override void _Draw()
	{
		float totalHeight = (_pointSize.Y * Count) + (Spacing * (Count - 1));
		float startY = -totalHeight / 2 + _pointSize.Y / 2;

		for (int i = 0; i < Count; i++)
		{
			Vector2 size = _pointSize;
			if (i == _currentIndex)
			{
				size = _pointFocusSize;
			}

			float yPos = startY + (_pointSize.Y + Spacing) * i;
			DrawRect(new Rect2(
				new Vector2(-size.X / 2, yPos - size.Y / 2),
				size
				), Color.Color8(255, 255, 255));
		}
	}
}
