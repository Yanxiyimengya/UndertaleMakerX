using Godot;
using Microsoft.VisualBasic;
using System;
[Tool]
[GlobalClass]
public partial class BattleMainArenaExpand : BattleRectangleArenaExpand
{
    BattleMainArenaExpand()
    {
        _size = new Vector2(140, 130);
    }
    public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
        Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
    {
        Vector2 borderSize = Vector2.One * BorderWidth;
        Rect2 _rect;

        _rect = new Rect2(
            -_size.X * 0.5F - borderSize.X,
            -_size.Y - borderSize.Y,
            _size.X + borderSize.X * 2F,
            _size.Y + borderSize.Y * 2F
        );
        RenderingServer.CanvasItemAddRect(borderRenderingItem, _rect, BorderColor);
        _rect = new Rect2(
            -_size.X * 0.5F,
            -_size.Y,
            _size.X,
            _size.Y
        );
        RenderingServer.CanvasItemAddRect(maskRenderingItem, _rect, ContentColor);
    }

    public override Vector2 GetRecentPointInArena(Vector2 point)
    {
        Vector2 minBounds = new Vector2(-_size.X * 0.5f, -_size.Y);
        Vector2 maxBounds = new Vector2(_size.X * 0.5f, 0);
        return point.Clamp(minBounds, maxBounds);
    }

    public override bool IsPointInArena(Vector2 point)
    {
        Rect2 arenaRect = new Rect2(-_size.X * 0.5F, -_size.Y, _size.X, _size.Y);
        return arenaRect.HasPoint(point);
    }

    public override bool IsSegmentInArena(Vector2 from, Vector2 to)
    {
        Vector2 borderSize = Vector2.One * BorderWidth;
        Rect2 arenaRect = new Rect2(
            -_size.X * 0.5F + borderSize.X,
            -_size.Y + borderSize.Y,
            _size.X - borderSize.X * 2F,
            _size.Y - borderSize.Y * 2F
        );

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
            if (Geometry2D.SegmentIntersectsSegment(from, to, rectCorners[i], rectCorners[(i + 1) % 4]).VariantType != Variant.Type.Nil)
            {
                return true;
            }
        }

        return false;
    }
}
