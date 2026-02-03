using Godot;
using System;  

[Tool]
[GlobalClass]
public partial class BattlePolygonArenaExpand : BattleArenaExpand
{
    [Export]
    public Vector2[] Vertices
    {
        get => _vertices;
        set
        {
            if (_vertices != value)
            {
                _vertices = value;
                _borderVertices = PolygonBuildTool.ExpandPolygon(_vertices, BorderWidth);
                _bBox = PolygonBuildTool.GetBBox(_vertices);

                _borderColors = new Color[_borderVertices.Length];
                for (int i = 0; i < _borderColors.Length; i++)
                    _borderColors[i] = BorderColor;

                _borderColors = new Color[Vertices.Length];
                for (int i = 0; i < _borderColors.Length; i++)
                    _borderColors[i] = ContentColor;
            }
        }
    }

    private Vector2[] _vertices;
    private Vector2[] _borderVertices;
    private Rect2 _bBox;
    private Color[] _borderColors;
    private Color[] _contentColors;

    public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
        Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
    {
        if (Vertices.Length < 3) return;
        RenderingServer.CanvasItemAddPolygon(borderRenderingItem, _borderVertices, _borderColors);
        RenderingServer.CanvasItemAddPolygon(maskRenderingItem, Vertices, _contentColors);
    }

    public override bool IsPointInArena(Vector2 point)
    {
        return _bBox.HasPoint(point) && Geometry2D.IsPointInPolygon(point, Vertices);
    }

    public override bool IsSegmentInArena(Vector2 from, Vector2 to)
    {
        if (IsPointInArena(from) || IsPointInArena(to))
        {
            return true;
        }

        for (int i = 0; i < Vertices.Length; i++)
        {
            var v1 = Vertices[i];
            var v2 = Vertices[(i + 1) % Vertices.Length];

            if (Geometry2D.SegmentIntersectsSegment(from, to, v1, v2).VariantType != Variant.Type.Nil)
            {
                return true;
            }
        }

        return false;
    }

    public override Vector2 GetRecentPointInArena(Vector2 pos)
    {
        Vector2 result = pos;
        float minDistSq = float.MaxValue;

        for (int i = 0; i < Vertices.Length; i++)
        {
            var v1 = Vertices[i];
            var v2 = Vertices[(i + 1) % Vertices.Length];

            var edgePoint = Geometry2D.GetClosestPointToSegment(pos, v1, v2);
            var distSq = pos.DistanceSquaredTo(edgePoint);

            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                result = edgePoint;
            }
        }

        return result;
    }
}
