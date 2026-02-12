using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class BaseBattleProjectile : GameSprite2D, IObjectPoolObject
{
	public enum PrecisionCollisionMode { FullTexture = 0, UsedRect = 1, Precise = 2 }

	[Export(PropertyHint.Enum, "FullTexture|UsedRect|Precise")]
	public PrecisionCollisionMode CollisionMode
	{
		get => collisionMode;
		set
		{
			if (collisionMode != value)
			{
				collisionMode = value;
				if (IsInsideTree()) UpdateCollisionShapes();
			}
		}
	}
	[Export]
	public override Texture2D[] Textures
	{
		get => _textures;
		set
		{
			_textures = value;
			Frame = 0;
			UpdateCollisionShapes();
			OnFrameChanged();
		}
	}
	[Export] public float PreciseEpsilon = 1.0f;
	[Export] public float Damage = 1f;
	[Export] public bool DestroyOnTurnEnd = true;
	[Export]
	public bool Enabled
	{
		get => _enabled;
		set
		{
			if (_enabled != value)
			{
				_enabled = value;
				if (value)
				{
					ProcessMode = ProcessModeEnum.Inherit;
					SetPhysicsProcess(value);
				}
				else
				{
					ProcessMode = ProcessModeEnum.Disabled;
					SetPhysicsProcess(value);
				}
			}
		}
	}
	public bool UseMask
	{
		get => _useMask;
		set
		{
			var projectileController = UtmxBattleManager.GetBattleProjectileController();
			var targetParent = value ? projectileController.ArenaMask : projectileController.ProjectilesNode;
			if (IsInsideTree()) { this.CallDeferred("reparent", targetParent); }
			else { targetParent.AddChild(this); }
			_useMask = value;
		}
	}

	private PrecisionCollisionMode collisionMode = PrecisionCollisionMode.UsedRect;
	private readonly List<List<Node2D>> _collisionShapesList = new();
	private Area2D _area;
	private int _prevFrame = -1;
	private bool _textureChangedConnected = false;
	private bool _frameChangedConnected = false;
	private bool _enabled = false;
	private bool _useMask;
	private readonly Dictionary<Image, List<Vector2[]>> _polygonCache = new();

	public BaseBattleProjectile()
	{
		_area = new Area2D();
		if (_area != null)
		{
			AddChild(_area, false, InternalMode.Front);
			_area.CollisionLayer = (int)UtmxBattleManager.BattleCollisionLayers.Player;
			_area.CollisionMask = (int)(
				UtmxBattleManager.BattleCollisionLayers.Projectile |
				UtmxBattleManager.BattleCollisionLayers.Player);
		}

		if (!_frameChangedConnected)
		{
			Connect(AnimatedSprite2D.SignalName.FrameChanged, Callable.From(OnFrameChanged));
			_frameChangedConnected = true;
		}
		if (!_textureChangedConnected)
		{
			Connect(GameSprite2D.SignalName.TextureChanged, Callable.From(UpdateCollisionShapes));
			_textureChangedConnected = true;
		}
	}
	public override void _Notification(int what)
	{
		if (what == NotificationPredelete)
		{
			if (_area != null)
			{
				_area.QueueFree();
				_area = null;
			}
			if (_frameChangedConnected)
			{
				Disconnect(AnimatedSprite2D.SignalName.FrameChanged, Callable.From(OnFrameChanged));
				_frameChangedConnected = false;
			}
			if (_textureChangedConnected)
			{
				Disconnect(GameSprite2D.SignalName.TextureChanged, Callable.From(UpdateCollisionShapes));
				_textureChangedConnected = false;
			}
			ClearCollisionShapes();
			_polygonCache.Clear();
		}
	}

	public override void Awake()
	{
		base.Awake();
		Enabled = true;
	}

	public override void Disabled()
	{
		base.Disabled();
		Enabled = false;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (!Enabled) return;
		if (IsCollideWithThePlayer())
		{
			var soul = UtmxBattleManager.GetBattlePlayerController()?.PlayerSoul;
			if (soul != null) OnHitPlayer(soul);
		}
	}

	public override void Destroy()
	{
		if (UtmxBattleManager.IsInBattle())
		{
			UtmxBattleManager.GetBattleProjectileController().DeleteProjectile(this);
		}
		else
		{
			QueueFree();
		}
	}

	private void OnFrameChanged()
	{
		if (_prevFrame != Frame)
		{
			if (_prevFrame >= 0 && _prevFrame < _collisionShapesList.Count)
				SetShapesDisabled(_collisionShapesList[_prevFrame], true);
		}
		if (Frame >= 0 && Frame < _collisionShapesList.Count)
			SetShapesDisabled(_collisionShapesList[Frame], false);
		_prevFrame = Frame;
	}

	public virtual void OnHitPlayer(BattlePlayerSoul playerSoul)
	{
		if (playerSoul != null)
			UtmxPlayerDataManager.Hurt(Damage);
	}

	private void ClearCollisionShapes()
	{
		foreach (var shapes in _collisionShapesList)
			foreach (var shape in shapes)
				if (IsInstanceValid(shape)) shape.QueueFree();
		_collisionShapesList.Clear();
	}

	private bool IsCollideWithThePlayer()
	{
		if (_area == null) return false;
		foreach (Area2D area in _area.GetOverlappingAreas())
		{
			if (area is BattlePlayerSoulHitBox) return true;
		}
		return false;
	}

	private void UpdateCollisionShapes()
	{
		ClearCollisionShapes();
		if (Textures == null || Textures.Length == 0) return;

		var newLists = new List<List<Node2D>>(Textures.Length);
		foreach (var texture in Textures)
		{
			var img = texture?.GetImage();
			var list = GenerateCollisionShapes(img);
			foreach (var shape in list)
			{
				SetCollisionShapeDisabled(shape, true);
				_area.AddChild(shape);
			}
			newLists.Add(list);
		}
		_collisionShapesList.AddRange(newLists);
		OnFrameChanged();
	}

	private List<Node2D> GenerateCollisionShapes(Image img)
	{
		var shapes = new List<Node2D>();
		if (img == null) return shapes;

		var textureSize = img.GetSize();
		var textureCenter = -textureSize / 2;

		switch (collisionMode)
		{
			case PrecisionCollisionMode.FullTexture:
				shapes.Add(CreateRectangleShape(textureSize, Vector2.Zero));
				break;

			case PrecisionCollisionMode.UsedRect:
				{
					var usedRect = img.GetUsedRect();
					if (usedRect.Size.X > 0 && usedRect.Size.Y > 0)
					{
						var usedCenter = usedRect.Position + usedRect.Size / 2;
						shapes.Add(CreateRectangleShape(usedRect.Size, usedCenter + textureCenter));
					}
					break;
				}

			case PrecisionCollisionMode.Precise:
				{
					List<Vector2[]> polys;
					if (! _polygonCache.TryGetValue(img, out polys))
					{
						var bitmap = new Bitmap();
						bitmap.CreateFromImageAlpha(img);
						polys = bitmap.OpaqueToPolygons(new Rect2I(Vector2I.Zero, textureSize), PreciseEpsilon).ToList();
						_polygonCache[img] = polys;
					};

					if (polys.Count == 0)
					{
						Rect2 usedRect = img.GetUsedRect();
						if (usedRect.Size.X > 0 && usedRect.Size.Y > 0)
						{
							var usedCenter = usedRect.Position + usedRect.Size / 2;
							shapes.Add(CreateRectangleShape(usedRect.Size, usedCenter + textureCenter));
						}
					}
					else
					{
						foreach (var polygon in polys)
							shapes.Add(CreateCollisionPolygon(polygon, textureCenter));
					}
					break;
				}
		}

		return shapes;
	}

	private CollisionShape2D CreateRectangleShape(Vector2 size, Vector2 offset)
	{
		var shape = new RectangleShape2D { Size = size };
		return new CollisionShape2D
		{
			Shape = shape,
			Transform = new Transform2D(0, offset)
		};
	}

	private CollisionPolygon2D CreateCollisionPolygon(Vector2[] points, Vector2 offset)
	{
		if (points == null || points.Length < 3) return null;

		return new CollisionPolygon2D
		{
			Polygon = points,
			BuildMode = CollisionPolygon2D.BuildModeEnum.Solids,
			Transform = new Transform2D(0, offset)
		};
	}

	private void SetShapesDisabled(IReadOnlyList<Node2D> shapes, bool disabled)
	{
		foreach (var shape in shapes)
		{
			if (!IsInstanceValid(shape)) continue;
			SetCollisionShapeDisabled(shape, disabled);
		}
	}

	private void SetCollisionShapeDisabled(Node2D shape, bool disabled)
	{
		if (shape is CollisionShape2D cs)
			cs.Disabled = disabled;
		else if (shape is CollisionPolygon2D cp)
			cp.Disabled = disabled;
	}
}
