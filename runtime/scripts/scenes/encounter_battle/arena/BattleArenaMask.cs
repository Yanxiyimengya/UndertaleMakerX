using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleArenaMask : Node2D
{
	private BattleArenaGroup _arenaGroup;
	
	public BattleArenaMask()
	{
		ClipChildren = ClipChildrenMode.Only;
	}
	public override void _EnterTree()
	{
		_arenaGroup = null;
		if (GetParent() is BattleArenaGroup parentNode)
		{
			_arenaGroup = parentNode;
		}
	}
	
	public override void _Process(double delta)
	{
		if (_arenaGroup != null)
		{
			Rid _canvasItem = GetCanvasItem();
			RenderingServer.CanvasItemClear(_canvasItem);
			RenderingServer.CanvasItemAddSetTransform(_canvasItem, _arenaGroup.CameraTransform);
			RenderingServer.CanvasItemAddTextureRect(_canvasItem, GetViewportRect(),
					_arenaGroup.GetMaskViewportTexture());
		}
	}
}
