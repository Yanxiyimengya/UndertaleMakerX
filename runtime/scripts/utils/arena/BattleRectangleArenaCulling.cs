using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleRectangleArenaCulling : BattleArenaCulling
{
	[Export]
	public Vector2 size = new Vector2(140, 140);
   
	public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
		Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
	{
		Vector2 borderSize = Vector2.One * BorderWidth;
		Rect2 _rect;

		_rect = new Rect2(-size * 0.5F - borderSize, size + borderSize * 2F);
		RenderingServer.CanvasItemAddRect(maskCullingCanvasItem, _rect, Colors.White);
		// 反过来向遮罩层绘制

		_rect = new Rect2(-size * 0.5F, size);
		RenderingServer.CanvasItemAddRect(borderCullingCanvasItem, _rect, Colors.Black);
	}
}
