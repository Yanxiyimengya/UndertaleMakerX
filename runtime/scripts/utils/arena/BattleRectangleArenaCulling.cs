using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleRectangleArenaCulling : BattleArenaCulling
{
    [Export]
    public Vector2 Size
    {
        get => _size;
        set
        {
            if (_size != value)
            {
                _size = value;
                IsDirty = true;
                UpdateCollisionShape(_shape);
            }

        }
    }

    private Vector2 _size = new Vector2(140, 140);

    public override void DrawFrame(Rid borderRenderingItem, Rid maskRenderingItem,
		Rid borderCullingCanvasItem, Rid maskCullingCanvasItem)
	{
		Vector2 borderSize = Vector2.One * BorderWidth;
		Rect2 _rect;

		_rect = new Rect2(-_size * 0.5F - borderSize, _size + borderSize * 2F);
		RenderingServer.CanvasItemAddRect(maskCullingCanvasItem, _rect, Colors.White);

		_rect = new Rect2(-_size * 0.5F, _size);
		RenderingServer.CanvasItemAddRect(borderCullingCanvasItem, _rect, Colors.Black);
	}

    protected override Rid GenerateCollisionShape()
    {
        return PhysicsServer2D.RectangleShapeCreate();
    }

    protected override void UpdateCollisionShape(Rid shape)
    {
        Vector2 borderSize = Vector2.One * BorderWidth * 2F;
        PhysicsServer2D.ShapeSetData(shape, (_size+borderSize) / 2F);
    }
}
