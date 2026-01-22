using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleCircleArenaCulling : BattleArenaCulling
{
	[Export]
	public float radius = 70f;

	public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
		Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
	{
		RenderingServer.CanvasItemAddCircle(
			borderCullingCanvasItem,
			Vector2.Zero,
			radius - BorderWidth,
			Colors.White
		);

		RenderingServer.CanvasItemAddCircle(
			maskCullingCanvasItem,
			Vector2.Zero,
			radius,
			Colors.White
		);
	}
}
