using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleArenaMask : Node2D
{
	public BattleArenaMask()
	{
		ClipChildren = ClipChildrenMode.Only;
	}
	public override void _Process(double delta)
	{
		if (GetParent() is BattleArenaGroup _arenaGroup)
		{
			Rid _canvasItem = GetCanvasItem();
			RenderingServer.CanvasItemClear(_canvasItem);
			RenderingServer.CanvasItemAddSetTransform(_canvasItem, _arenaGroup.CameraTransform);
			RenderingServer.CanvasItemAddTextureRect(_canvasItem, GetViewportRect(),
					_arenaGroup.GetMaskViewportTexture());
		}
	}
}
