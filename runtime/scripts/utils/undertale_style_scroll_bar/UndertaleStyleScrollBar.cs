using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class UndertaleStyleScrollBar : Control
{
	[Export]
	public int Count
	{
		get => count;
		set
		{
			count = Math.Max(0, value); // 防止负数
			QueueRedraw();
		}
	}
	[Export]
	public int CurrentIndex // 修正拼写错误：CurrnetIndex → CurrentIndex
	{
		get => currentIndex;
		set
		{
			currentIndex = Math.Clamp(value, 0, Count - 1); // 边界保护
			QueueRedraw();
		}
	}
	[Export]
	public Vector2 PointSize
	{
		get => pointSize;
		set
		{
			pointSize = value;
			QueueRedraw();
		}
	}
	[Export]
	public Vector2 PointFocusSize
	{
		get => pointFocusSize;
		set
		{
			pointFocusSize = value;
			QueueRedraw();
		}
	}
	[Export]
	public float Spacing
	{
		get => spacing;
		set
		{
			spacing = value;
			QueueRedraw();
		}
	}

	[Export]
	public float PageSize = 3;

	[Export]
	public Texture2D ArrowTexture;

	private int count = 0;
	private int currentIndex = 0;
	private Vector2 pointSize = new Vector2(4 , 4);
	public Vector2 pointFocusSize = new Vector2(10, 10);
	private float spacing = 6;
	private bool inTop = false;
	private bool inBottom = false;
	private Rid topArrow;
	private Rid bottomArrow;
	private double arrowAnimTimer = 0.0;
	private double arrowAnimOffset = 0.0;
	private Tween tween;

	public override void _EnterTree()
	{
		topArrow = RenderingServer.CanvasItemCreate();
		RenderingServer.CanvasItemSetParent(topArrow, GetCanvasItem());
		bottomArrow = RenderingServer.CanvasItemCreate();
		RenderingServer.CanvasItemSetParent(bottomArrow, GetCanvasItem());

		if (ArrowTexture != null)
		{
			RenderingServer.CanvasItemAddTextureRect(topArrow, new Rect2(-ArrowTexture.GetSize() / 2, ArrowTexture.GetSize()), ArrowTexture.GetRid());
			RenderingServer.CanvasItemAddTextureRect(bottomArrow, new Rect2(-ArrowTexture.GetSize() / 2, ArrowTexture.GetSize()), ArrowTexture.GetRid());
		}
	}

	public override void _ExitTree()
	{
		if (topArrow.IsValid)
			RenderingServer.FreeRid(topArrow);
		if (bottomArrow.IsValid)
			RenderingServer.FreeRid(bottomArrow);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		inTop = currentIndex <= 0;
		inBottom = currentIndex >= Math.Max(0, Count - PageSize);

		if (Count > PageSize && ArrowTexture != null)
		{
			float halfCount = Count / 2.0f;
			float totalPointHeight = pointSize.Y * Count;
			float totalSpacing = Spacing * (Count - 1);
			float centerOffset = (totalPointHeight + totalSpacing) / 2;

			float topY = -centerOffset + pointSize.Y / 2 + Spacing / 2 - (float)arrowAnimOffset;
			RenderingServer.CanvasItemSetVisible(topArrow, !inTop);
			RenderingServer.CanvasItemSetTransform(topArrow,
				Transform2D.Identity.Translated(new Vector2(0F, topY - 10)));

			float bottomY = centerOffset - pointSize.Y / 2 - Spacing / 2 + (float)arrowAnimOffset;
			RenderingServer.CanvasItemSetVisible(bottomArrow, !inBottom);
			RenderingServer.CanvasItemSetTransform(bottomArrow,
				Transform2D.Identity.Scaled(new Vector2(1F, -1F)).Translated(new Vector2(0F, bottomY + 10)));

			if (arrowAnimTimer > 0)
			{
				arrowAnimTimer -= delta;
			}
			else
			{
				arrowAnimTimer = 1.0F;
				if (tween != null && tween.IsRunning())
				{
					tween.Kill();
				}
				tween = GetTree().CreateTween();
				tween.TweenProperty(this, "arrowAnimOffset", 5, 0.42).From(0.0);
			}
		}
		else
		{
			if (topArrow.IsValid)
				RenderingServer.CanvasItemSetVisible(topArrow, false);
			if (bottomArrow.IsValid)
				RenderingServer.CanvasItemSetVisible(bottomArrow, false);
		}
	}

	public override void _Draw()
	{
		float totalHeight = (pointSize.Y * Count) + (Spacing * (Count - 1));
		float startY = -totalHeight / 2 + pointSize.Y / 2;

		for (int i = 0; i < Count; i++)
		{
			Vector2 size = pointSize;
			if (i == currentIndex)
			{
				size = PointFocusSize;
			}

			float yPos = startY + (pointSize.Y + Spacing) * i;
			DrawRect(new Rect2(
				new Vector2(-size.X / 2, yPos - size.Y / 2),
				size
				), Color.Color8(255, 255, 255));
		}
	}
}
