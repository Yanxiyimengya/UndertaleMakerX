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
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (IsInsideTree() && Visible)
		{
			BattleArenaGroup _arenaGroup = GetParent() as BattleArenaGroup;
			if (_arenaGroup != null && _arenaGroup.Visible)
			{
				Rid _canvasItem = GetCanvasItem();
				RenderingServer.CanvasItemClear(_canvasItem);
				RenderingServer.CanvasItemAddSetTransform(_canvasItem, _arenaGroup.CameraTransform);
				RenderingServer.CanvasItemAddTextureRect(_canvasItem, GetViewportRect(),
						_arenaGroup.GetMaskViewportTexture());
			}
		}
	}
}
