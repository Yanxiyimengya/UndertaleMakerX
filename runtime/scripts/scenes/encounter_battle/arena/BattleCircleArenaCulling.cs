using Godot;
using System;
using System.Threading.Tasks;

[Tool]
[GlobalClass]
public partial class BattleCircleArenaCulling : BattleArenaCulling
{
    [Export]
    public float Radius
    {
        get => _radius;
        set
        {
            if (_radius != value)
            {
                _radius = value;
                UpdateCollisionShape(_shape);
            }
        }
    }

    private float _radius = 70f;
    public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
        Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
    {
        RenderingServer.CanvasItemAddCircle(
            borderCullingCanvasItem,
            Vector2.Zero,
            _radius,
            Colors.White
        );

        RenderingServer.CanvasItemAddCircle(
            maskCullingCanvasItem,
            Vector2.Zero,
            _radius + BorderWidth,
            Colors.White
        );
    }
    protected override Rid GenerateCollisionShape()
    {
        return PhysicsServer2D.CircleShapeCreate();
    }

    protected override void UpdateCollisionShape(Rid shape)
    {
        PhysicsServer2D.ShapeSetData(_shape, _radius + BorderWidth);
    }

    Tween _tween;
    public async Task Resize(double radius, double duration = 0.4)
    {
        if (_tween != null && _tween.IsRunning())
            _tween.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(this, "Radius", radius, duration);
        await ToSignal(_tween, Tween.SignalName.Finished);
    }
}
