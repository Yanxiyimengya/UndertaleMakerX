using Godot;
using System;
using System.Threading.Tasks;

[Tool]
[GlobalClass]
public partial class BattleCircleArenaExpand : BattleArenaExpand
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
            }
        }
    }

    private float _radius = 70f;

    public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
        Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
    {
        RenderingServer.CanvasItemAddCircle(
            borderRenderingItem,
            Vector2.Zero,
            _radius + BorderWidth,
            BorderColor
        );

        RenderingServer.CanvasItemAddCircle(
            maskRenderingItem,
            Vector2.Zero,
            _radius,
            ContentColor
        );
    }
    public override Vector2 GetRecentPointInArena(Vector2 pos)
    {
        return pos.Length() > _radius ? pos.Normalized() * _radius : pos;
    }
    public override bool IsPointInArena(Vector2 point)
    {
        return Geometry2D.IsPointInCircle(point, Vector2.Zero, _radius);
    }
    public override bool IsSegmentInArena(Vector2 from, Vector2 to)
    {
        float intersection = Geometry2D.SegmentIntersectsCircle(from, to, Vector2.Zero, _radius);
        return intersection != -1;
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
