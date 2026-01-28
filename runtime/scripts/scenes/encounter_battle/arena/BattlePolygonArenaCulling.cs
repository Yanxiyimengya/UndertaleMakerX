using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

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
                IsDirty = true;
                _borderVertices = PolygonBuildTool.ExpandPolygon(_vertices, BorderWidth);
                UpdateCollisionShape(_shape);
            }
        }
    }

    ~BattlePolygonArenaCulling()
    {
        foreach (Rid shapeRid in _convexShapes)
            PhysicsServer2D.FreeRid(shapeRid);
        _convexShapes.Clear();
    }

    private Vector2[] _vertices;
    private Vector2[] _borderVertices;
    private List<Rid> _convexShapes = new List<Rid>();

    public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
        Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
    {
        if (Vertices.Length < 3) return;

        var borderColors = new Color[_borderVertices.Length];
        for (int i = 0; i < borderColors.Length; i++)
        {
            borderColors[i] = Colors.White;
        }
        RenderingServer.CanvasItemAddPolygon(maskCullingCanvasItem, _borderVertices, borderColors);

        var contentColors = new Color[Vertices.Length];
        for (int i = 0; i < contentColors.Length; i++)
        {
            contentColors[i] = Colors.White;
        }
        RenderingServer.CanvasItemAddPolygon(borderCullingCanvasItem, Vertices, contentColors);
    }

    protected override Rid GenerateCollisionShape()
    {
        return new Rid();
    }

    protected override void UpdateCollisionShape(Rid _)
    {
        foreach (Rid shapeRid in _convexShapes)
            PhysicsServer2D.FreeRid(shapeRid);
        _convexShapes.Clear();
        PhysicsServer2D.BodyClearShapes(_arenaPhysicBody);
        if (_borderVertices.Length < 3) return;
        Godot.Collections.Array<Vector2[]> convexPolygons = Geometry2D.DecomposePolygonInConvex(_borderVertices);
        foreach (Vector2[] convexPolygon in convexPolygons)
        {
            Rid convexShape = PhysicsServer2D.ConvexPolygonShapeCreate();
            PhysicsServer2D.ShapeSetData(convexShape, convexPolygon);
            PhysicsServer2D.BodyAddShape(_arenaPhysicBody, convexShape);
            _convexShapes.Add(convexShape);
        }
    }
}
