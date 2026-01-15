using Godot;
using System;

[Tool]
[GlobalClass]
public partial class BattleArena : Node2D
{
	[Export]
	Vector2 Size { get => size;
		set {
			size = value;
			QueueRedraw();
		}
	}
	[Export]
	int borderWidth = 5;

	[Export]
	StyleBox borderStyleBox;
	[Export]
	StyleBox contentStyleBox;
	
	[Export]
	CollisionShape2D left;
	[Export]
	CollisionShape2D right;
	[Export]
	CollisionShape2D top;
	[Export]
	CollisionShape2D bottom;

	Vector2 size = new Vector2(140, 140);

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Engine.IsEditorHint())
		{
			QueueRedraw();
		}

		float offset = (float)borderWidth / 2;

		left.Position = new Vector2(-size.X / 2 - offset, 0);
		(left.Shape as RectangleShape2D).Size = new Vector2 (borderWidth,
			size.Y + borderWidth * 2);

		right.Position = new Vector2(size.X / 2 + offset, 0);
		(right.Shape as RectangleShape2D).Size = new Vector2(borderWidth,
			size.Y + borderWidth * 2);

		top.Position = new Vector2(0, -size.Y / 2 - offset);
		(top.Shape as RectangleShape2D).Size = new Vector2(
			size.X + borderWidth * 2, 5);

		bottom.Position = new Vector2(0, size.Y / 2 + offset);
		(bottom.Shape as RectangleShape2D).Size = new Vector2(
			size.X + borderWidth * 2, 5);
	}

	public override void _Draw()
	{
		base._Draw();
		Rid canvasItemRid = GetCanvasItem();

		Rect2 borderRect = new Rect2(-size / 2 - Vector2.One * borderWidth, 
			size + Vector2.One * borderWidth * 2);
		Rect2 contentRect = new Rect2(-size / 2,
			size);

		if (borderStyleBox == null)
		{
			RenderingServer.CanvasItemAddRect(canvasItemRid, borderRect, Color.Color8(255, 255, 255));
		}
		else
		{
			borderStyleBox.Draw(canvasItemRid, borderRect);
		}


		if (contentStyleBox == null)
		{
			RenderingServer.CanvasItemAddRect(canvasItemRid, contentRect, Color.Color8(0, 0, 0));
		}
		else
		{
			contentStyleBox.Draw(canvasItemRid, contentRect);
		}

	}

}
