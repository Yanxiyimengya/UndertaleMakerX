using Godot;
using System;

[GlobalClass]
public abstract partial class BaseBattleArena : Node2D
{
	[Export]
	public float BorderWidth = 5.0F;
	public bool IsDirty = true;
    public bool Enabled = true;

    public abstract void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
        Rid borderCullingCanvasItem, Rid maskCullingCanvasItem);
}