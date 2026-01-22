using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattlePolygonArenaCulling : BattleArenaCulling
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
                _borderVertices = ArenaPolygonBorderTool.GetBorderPolygon(_vertices, -BorderWidth);
            }
        }
    }

    private Vector2[] _vertices;
    private Vector2[] _borderVertices;

    public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
        Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
    {
        if (Vertices.Length < 3) return;

        var borderColors = new Color[_borderVertices.Length];
        for (int i = 0; i < borderColors.Length; i++)
        {
            borderColors[i] = Colors.White;
        }
        RenderingServer.CanvasItemAddPolygon(borderCullingCanvasItem, _borderVertices, borderColors);

        var contentColors = new Color[Vertices.Length];
        for (int i = 0; i < contentColors.Length; i++)
        {
            contentColors[i] = Colors.White;
        }
        RenderingServer.CanvasItemAddPolygon(maskCullingCanvasItem, Vertices, contentColors);
    }
}
