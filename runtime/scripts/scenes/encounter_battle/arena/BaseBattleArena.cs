using Godot;
using System;

[GlobalClass]
public abstract partial class BaseBattleArena : Node2D
{
    [Export]
    public float BorderWidth = 5.0F;
    [Export]
    public virtual bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            Visible = value;
        }
    }
    public bool IsDirty = true;

    protected bool _enabled = true;

    public abstract void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
        Rid borderCullingCanvasItem, Rid maskCullingCanvasItem);
}