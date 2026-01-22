using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleArenaMask : Node2D
{
	public override void _Ready()
	{
		ClipChildren = ClipChildrenMode.Only;
	}

	public override void _Draw()
	{
		BattleArenaGroup _arenaGroup = GetParent() as BattleArenaGroup;
		if (_arenaGroup != null)
		{
			Rid _canvasItem = GetCanvasItem();
			RenderingServer.CanvasItemAddTextureRect(_canvasItem, GetViewportRect(),
					_arenaGroup.GetMaskViewportTexture());
		}
	}
}
