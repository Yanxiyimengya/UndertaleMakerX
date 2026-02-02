using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BaseBattleProjectile : Area2D
{
	public enum CollisionPrecision
	{
		FullTexture,
		UsedRect,
		Precise,
	}

	[Export]
	public Texture2D Texture
	{
		get => _texture;
		set
		{
			if (_texture != value)
			{
				_texture = value;
				_image = _texture != null ? _texture.GetImage() : null;
				UpdateCollisionShape();
			}
		}
	}

	[Export(PropertyHint.Enum, "FullTexture|UsedRect|Precise")]
	public CollisionPrecision PreciseCollision
	{
		get => _preciseCollision;
		set
		{
			if (_preciseCollision != value)
			{
				_preciseCollision = value;
				UpdateCollisionShape();
			}
		}
	}
	[Export]
	public float PreciseEpsilon = 0.5F;
	[Export]
	public float Damage = 1F;


	private Texture2D _texture;
	private Image _image;
	private Vector2[] _polygonVertexs = [];
	private CollisionPrecision _preciseCollision = CollisionPrecision.UsedRect;
	private List<Rid> _collisionShapes = new();

	public BaseBattleProjectile()
	{
		CollisionLayer = (int)UtmxBattleManager.BattleCollisionLayers.Projectile;
		CollisionMask = (int)UtmxBattleManager.BattleCollisionLayers.Player;
		AreaEntered += _BaseBattleProjectileAreaEntered;
	}

	private void _BaseBattleProjectileAreaEntered(Area2D area)
	{
		if (area is BattlePlayerSoulHitBox hitBox)
		{
			OnHitPlayer(hitBox.GetParent() as BattlePlayerSoul);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
		if (soul != null && soul.CanBeHurt() && IsCollideWithThePlayer())
		{
			OnHitPlayer(soul);
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		ClearCollisionShapes();
	}

	public override void _Ready()
	{
		base._Ready();
		UpdateCollisionShape();
	}
	public override void _Draw()
	{
		if (Texture == null) return;
		DrawTexture(Texture, -Texture.GetSize() / 2);
	}

	public void UpdateCollisionShape()
	{
		if (Texture == null || _image == null)
		{
			ClearCollisionShapes();
			return;
		}
		Rid bodyRid = GetRid();
		if (!bodyRid.IsValid)
		{
			return;
		}

		ClearCollisionShapes();

		Vector2 textureSize = Texture.GetSize();
		Vector2 shapeOffset = -textureSize / 2;

		switch (_preciseCollision)
		{
			case CollisionPrecision.FullTexture:
				Rid fullRectShape = PhysicsServer2D.RectangleShapeCreate();
				PhysicsServer2D.ShapeSetData(fullRectShape, Texture.GetSize() * 0.5F);
				PhysicsServer2D.AreaAddShape(bodyRid, fullRectShape);
				_collisionShapes.Add(fullRectShape);
				break;

			case CollisionPrecision.UsedRect:
				Rect2I usedRect = _image.GetUsedRect();
				if (usedRect.Size.X > 0 && usedRect.Size.Y > 0)
				{
					Rid uesdRectShape = PhysicsServer2D.RectangleShapeCreate();
					PhysicsServer2D.ShapeSetData(uesdRectShape, (Vector2)usedRect.Size * 0.5F);
					Vector2 textureCenter = -textureSize / 2;
					Vector2 usedRectCenter = usedRect.Position + usedRect.Size / 2;
					PhysicsServer2D.AreaAddShape(bodyRid, uesdRectShape, new Transform2D(0, usedRectCenter + textureCenter));
					_collisionShapes.Add(uesdRectShape);
				}
				break;
			case CollisionPrecision.Precise:
				Bitmap bitmap = new Bitmap();
				bitmap.CreateFromImageAlpha(_image);
				Array<Vector2[]> polygons = bitmap.OpaqueToPolygons(
					new Rect2I(Vector2I.Zero, (Vector2I)textureSize), PreciseEpsilon);
				foreach (Vector2[] polygon in polygons)
				{
					Array<Vector2[]> convexPolygons = Geometry2D.DecomposePolygonInConvex(polygon);
					foreach (Vector2[] convexPolygon in convexPolygons)
					{
						Vector2[] offsetConvexPolygon = new Vector2[convexPolygon.Length];
						for (int i = 0; i < convexPolygon.Length; i++)
						{
							offsetConvexPolygon[i] = convexPolygon[i] + shapeOffset;
						}
						Rid convexShape = PhysicsServer2D.ConvexPolygonShapeCreate();
						PhysicsServer2D.ShapeSetData(convexShape, offsetConvexPolygon);
						PhysicsServer2D.AreaAddShape(bodyRid, convexShape);
						_collisionShapes.Add(convexShape);
					}
				}
				break;
		}
	}
	private void ClearCollisionShapes()
	{
		Rid bodyRid = GetRid();
		if (!bodyRid.IsValid) return;

		foreach (Rid shapeRid in _collisionShapes)
		{
			if (shapeRid.IsValid)
			{
				PhysicsServer2D.FreeRid(shapeRid);
			}
		}
		PhysicsServer2D.AreaClearShapes(bodyRid);
		_collisionShapes.Clear();
	}

	public bool IsCollideWithThePlayer()
	{
		foreach (Area2D area in GetOverlappingAreas())
		{
			if (area is BattlePlayerSoulHitBox hitBox)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void OnHitPlayer(BattlePlayerSoul playerSoul)
	{
		playerSoul.Hurt(Damage);
	}
}
