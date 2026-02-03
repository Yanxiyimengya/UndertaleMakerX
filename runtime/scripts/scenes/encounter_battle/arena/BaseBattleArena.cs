using Godot;
using System;

[GlobalClass]
public abstract partial class BaseBattleArena : Node2D
{
    [Export(PropertyHint.Range, "0,200,0.1")]
    public float BorderWidth
    {
        get => _borderWidth;
        set
        {
            if (Mathf.IsEqualApprox(_borderWidth, value))
                return;
            _borderWidth = Mathf.Max(0f, value);
        }
    }

    [Export]
    public virtual bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value)
                return;
            _enabled = value;
            if (Visible != value)
                Visible = value;
        }
    }

    private float _borderWidth = 5.0f;
    protected bool _enabled = true;

    private Transform2D _lastGlobalTransform;
    private bool _lastVisible;
    private bool _initialized;

    public override void _Ready()
    {
        _lastGlobalTransform = GlobalTransform;
        _lastVisible = Visible;

        if (Visible != _enabled)
            Visible = _enabled;
        _initialized = true;
    }
    public abstract void DrawFrame(
        Rid borderRenderingItem,
        Rid maskRenderingItem,
        Rid borderCullingCanvasItem,
        Rid maskCullingCanvasItem
    );
}
