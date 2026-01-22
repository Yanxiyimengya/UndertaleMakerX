using Godot;
using Microsoft.VisualBasic;
using System;

[Tool]
[GlobalClass]
public partial class BattleRectangleArenaExpand : BattleArenaExpand
{
	[Export]
	public Vector2 size = new Vector2(140, 140);
	
	public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
		Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
	{
		Vector2 borderSize = Vector2.One * BorderWidth;
		Rect2 _rect;

		_rect = new Rect2(-size * 0.5F - borderSize, size + borderSize * 2F);
		RenderingServer.CanvasItemAddRect(borderRenderingItem, _rect, BorderColor);
		
		_rect = new Rect2(-size * 0.5F , size);
		RenderingServer.CanvasItemAddRect(maskRenderingItem, _rect, ContentColor);
	}

	public override Vector2 GetRecentPointInArena(Vector2 point)
	{
		Vector2 half = size * 0.5f;
		Vector2 minBounds = -half;
		Vector2 maxBounds = half;
		return point.Clamp(minBounds, maxBounds);
	}

	public override bool IsPointInArena(Vector2 point)
	{
		Vector2 borderSize = Vector2.One * BorderWidth;
		Rect2 arenaRect = new Rect2(-size * 0.5F, size);
		return arenaRect.HasPoint(point);
	}
	public override bool IsSegmentInArena(Vector2 from, Vector2 to)
	{
		Vector2 borderSize = Vector2.One * BorderWidth;
		Rect2 arenaRect = new Rect2(-size * 0.5F + borderSize, size - borderSize);

		if (arenaRect.HasPoint(from) || arenaRect.HasPoint(to))
		{
			return true;
		}

		Vector2[] rectCorners = {
			arenaRect.Position,
			new Vector2(arenaRect.Position.X + arenaRect.Size.X, arenaRect.Position.Y),
			new Vector2(arenaRect.Position.X + arenaRect.Size.X, arenaRect.Position.Y + arenaRect.Size.Y),
			new Vector2(arenaRect.Position.X, arenaRect.Position.Y + arenaRect.Size.Y)
		};

		for (int i = 0; i < 4; i++)
		{
			if (Geometry2D.SegmentIntersectsSegment(from, to, rectCorners[i], rectCorners[(i + 1) % 4]).VariantType is Variant.Type.Nil)
			{
				return true;
			}
		}

		return false;
	}
}
