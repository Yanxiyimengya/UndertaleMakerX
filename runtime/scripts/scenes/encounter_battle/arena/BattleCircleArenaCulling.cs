using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleCircleArenaCulling : BattleArenaCulling
{
	[Export]
	public float Radius
	{
		get => _radius;
		set
		{
			if (_radius != value)
			{
				_radius = value;
				IsDirty = true;
				UpdateCollisionShape(_shape);
			}
		}
	}

	private float _radius = 70f;
	public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
		Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
	{
		RenderingServer.CanvasItemAddCircle(
			borderCullingCanvasItem,
			Vector2.Zero,
			_radius,
			Colors.White
		);

		RenderingServer.CanvasItemAddCircle(
			maskCullingCanvasItem,
			Vector2.Zero,
			_radius + BorderWidth,
			Colors.White
		);
	}
	protected override Rid GenerateCollisionShape()
	{
		return PhysicsServer2D.CircleShapeCreate();
	}

	protected override void UpdateCollisionShape(Rid shape)
	{
		PhysicsServer2D.ShapeSetData(_shape, _radius + BorderWidth);
	}
}
