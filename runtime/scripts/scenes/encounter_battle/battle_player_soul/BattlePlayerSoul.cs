using Godot;
using System;
using System.Collections.Generic;
[GlobalClass]
public partial class BattlePlayerSoul : CharacterBody2D
{
	[Export]
	BattleArenaGroup ArenaGroup;
	[Export]
	public double CollisionRadius
	{
		get => _collisionRadius;
		set
		{
			if (_collisionRadius != value)
			{
				_collisionRadius = value;
				int count = 6;
				_checkPoints.Clear();
				for (float i = 0; i < count; i++)
				{
					_checkPoints.Add((float)value * Vector2.Down.Rotated((i / count) * MathF.Tau + MathF.PI));
				}
			}
		}
	}
	[Export]
	public bool EnabledCollision
	{
		get => _enabledCollision;
		set
		{
			_enabledCollision = value;
			if (value)
			{
				PhysicsServer2D.BodySetMode(GetRid(), PhysicsServer2D.BodyMode.Static);
				CollisionLayer = _collisionLayer;
				CollisionMask = _collisionMask;
			}
			else
			{
				PhysicsServer2D.BodySetMode(GetRid(), PhysicsServer2D.BodyMode.Kinematic);
				_collisionLayer = CollisionLayer;
				_collisionMask = CollisionMask;
				CollisionLayer = 0;
				CollisionMask = 0;
			}
		}
	}
	[Export]
	public bool EnabledHitBoxCollision
	{
		get => _enabledHitBoxCollision;
		set
		{
			_enabledHitBoxCollision = value;
			if (value)
			{
				HitBox.CollisionLayer = _collisionLayer;
				HitBox.CollisionMask = _collisionMask;
			}
			else
			{
				HitBox.CollisionLayer = 0;
				HitBox.CollisionMask = 0;
			}
		}
	}
	[Export]
	public bool Movable
	{
		get => _movable;
		set
		{
			_movable = value;
		}
	}

	[Export]
	public bool Freed
	{
		get => _freed;
		set
		{
			_freed = value;
			if (_freed)
			{
				EnabledCollision = false;
				Movable = false;
				Sprite.Play("free");
			}
		}
	}
	
	public Color SoulColor
	{
		get => Sprite.Modulate;
		set
		{
			Sprite.Modulate = value;
		}
	}

	public bool IsMoving { get => _isMoving; set => _isMoving = value; }

	[Export]
	public AnimatedSprite2D Sprite;
	[Export]
	public AnimationPlayer AnimPlayer;
	[Export]
	public BattlePlayerSoulHitBox HitBox;

	private uint _collisionLayer = 0;
	private uint _collisionMask = 0;
	private double _collisionRadius = 0;
	private bool _enabledCollision = false;
	private bool _enabledHitBoxCollision = true;
	private bool _movable = true;
	private bool _freed = false;
	private double _invincibleTimer = 0.0F;
	private bool _isMoving = false;
	private Vector2 _prevPosition = Vector2.Zero;
	private List<Vector2> _checkPoints = new List<Vector2>();

	public const float MOVE_SPEED = 130.0f;
	public BattlePlayerSoul()
	{
		_collisionLayer = (int)UtmxBattleManager.BattleCollisionLayers.Player;
		_collisionMask = (int)UtmxBattleManager.BattleCollisionLayers.Player;
		CollisionLayer = (int)UtmxBattleManager.BattleCollisionLayers.Player;
		CollisionMask = (int)UtmxBattleManager.BattleCollisionLayers.Player;
	}

	public override void _Ready()
	{
		_prevPosition = Position;
	}
	public override void _Process(double delta)
	{
		if (_invincibleTimer > 0)
		{
			_invincibleTimer -= delta;
			AnimPlayer.Play("hurt");
		}
		else
		{
			if (AnimPlayer.IsPlaying() && AnimPlayer.CurrentAnimation != "RESET")
			{
				AnimPlayer.Stop();
				AnimPlayer.Play("RESET");
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		ProcessMove(delta);
	}

	public void ProcessMove(double delta)
	{
		Vector2 targetPos = Position;
		if (_movable)
		{
			Vector2 inputDir = Input.GetVector("left", "right", "up", "down").Normalized().Rotated(GlobalRotation);
			targetPos = Position + inputDir * (float)delta *
				(Input.IsActionPressed("cancel") ? (MOVE_SPEED * 0.5F) : MOVE_SPEED);
			if (inputDir.X != 0 || inputDir.Y != 0)
			{
				TryMoveTo(targetPos);
			}
		}
		else MoveAndSlide();
		IsMoving = targetPos != _prevPosition;
		_prevPosition = targetPos;
	}

	private bool IsInsideArena(Vector2 center)
	{
		if (!_enabledCollision) return true;
		foreach (Vector2 offset in _checkPoints)
		{
			Vector2 worldPoint = center + offset;
			if (! ArenaGroup.IsPointInArenas(worldPoint, true))
			{
				return false;
			}
		}
		return true;
	}
	public bool IsOnArenaFloor()
	{
		if (!_enabledCollision) return true;
		float radius = (float)_collisionRadius + 2F;
		return	(!ArenaGroup.IsPointInArenas(GlobalPosition + new Vector2(-0.8F, 1).Normalized().Rotated(Rotation) * radius)) ||
				(!ArenaGroup.IsPointInArenas(GlobalPosition + new Vector2(0.8F, 1).Normalized().Rotated(Rotation) * radius)) ||
				(!ArenaGroup.IsPointInArenas(GlobalPosition + Vector2.Down.Normalized().Rotated(Rotation) * radius))
		|| IsOnFloor();
	}
	public bool IsOnArenaCeiling()
	{
		if (!_enabledCollision) return true;
		float radius = (float)_collisionRadius + 1.5F;
		return !ArenaGroup.IsPointInArenas(
			GlobalPosition + Vector2.Up.Rotated(Rotation) * radius)
		|| IsOnCeiling();
	}

	public void TryMoveTo(Vector2 targetPos)
	{
		Position = targetPos;
		Velocity = Vector2.Zero;
		if (! IsInsideArena(Position))
			Position = ArenaGroup.PushBackInside(Position, _checkPoints.ToArray(), 1.0F);
		MoveAndSlide();
	}

	public void Hurt(double damage, double invtime = -1)
	{
		if (_invincibleTimer <= 0)
		{
			UtmxPlayerDataManager.PlayerHp -= damage;
			if (invtime >= 0)
			{
				_invincibleTimer = invtime;
			}
			else
			{
				_invincibleTimer = UtmxPlayerDataManager.PlayerInvincibleTime;
			}
			if (GetViewport().GetCamera2D() is GameCamera camera)
				camera.StartShake(0.12f, new Vector2(2, 2), new Vector2(30, 30));
			UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("HURT"));
		}
	}

	public void SetInvincibleTimer(double timer = 0.0)
	{
		_invincibleTimer = timer;
	}
}
