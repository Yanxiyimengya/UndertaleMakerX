using Godot;  
using System;  
  
[Tool]  
[GlobalClass]  
public partial class BattleCircleArenaExpand : BattleArenaExpand  
{  
	[Export]  
	public float radius = 70f;  
	  
	public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
		Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
	{   
		RenderingServer.CanvasItemAddCircle(  
			borderRenderingItem,   
			Vector2.Zero,   
			radius + BorderWidth,   
			BorderColor  
		);  
		   
		RenderingServer.CanvasItemAddCircle(
			maskRenderingItem,   
			Vector2.Zero,   
			radius,   
			ContentColor  
		);  
	}
	public override Vector2 GetRecentPointInArena(Vector2 pos)
	{
		return pos.Length() > radius ? pos.Normalized() * radius : pos;
	}
	public override bool IsPointInArena(Vector2 point)  
	{  
		return Geometry2D.IsPointInCircle(point, Vector2.Zero, radius);  
	}  
	  
	public override bool IsSegmentInArena(Vector2 from, Vector2 to)  
	{  
		float intersection = Geometry2D.SegmentIntersectsCircle(from, to, Vector2.Zero, radius); 
		return intersection >= 0;  
	}  
}
