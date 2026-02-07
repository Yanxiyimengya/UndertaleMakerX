using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.CompilerServices.RuntimeHelpers;

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
				HitBox.CollisionLayer = _collisionLayer;
				HitBox.CollisionMask = _collisionMask;
			}
			else
			{
				PhysicsServer2D.BodySetMode(GetRid(), PhysicsServer2D.BodyMode.Kinematic);
				_collisionLayer = CollisionLayer;
				_collisionMask = CollisionMask;
				CollisionLayer = 0;
				CollisionMask = 0;
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

	[Export]
	public AnimatedSprite2D Sprite;
	[Export]
	public AnimationPlayer AnimPlayer;
	[Export]
	public BattlePlayerSoulHitBox HitBox;

	private uint _collisionLayer = 0;
	private uint _collisionMask = 0;
	private double _collisionRadius = 0;
	private bool _enabledCollision = true;
	private bool _movable = true;
	private bool _freed = false;
	private double _invincibleTimer = 0.0F;
	private List<Vector2> _checkPoints = new List<Vector2>();

	public const float MOVE_SPEED = 130.0f;
	public BattlePlayerSoul()
	{
		CollisionLayer = (int)UtmxBattleManager.BattleCollisionLayers.Player;
		CollisionMask = (int)UtmxBattleManager.BattleCollisionLayers.Player;
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
		if (_movable)
		{
			ProcessMove(delta);
		}
	}

	public void ProcessMove(double delta)
	{
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down").Normalized().Rotated(GlobalRotation);
		Vector2 targetPos = Position + inputDir * (float)delta * 
			(Input.IsActionPressed("cancel") ? (MOVE_SPEED * 0.5F) : MOVE_SPEED);
		TryMoveTo(targetPos);
        Velocity = Vector2.Zero;
        MoveAndSlide();
	}

	private bool IsInsideArena(Vector2 center)
	{
		if (!_enabledCollision) return true;
		foreach (Vector2 offset in _checkPoints)
		{
			Vector2 worldPoint = center + offset;
			if (!ArenaGroup.IsPointInArenas(worldPoint))
			{
				return false;
			}
		}
		return true;
	}

	public void TryMoveTo(Vector2 targetPos)
	{
		if (ArenaGroup.EnabledArenaCount > 0 && !IsInsideArena(targetPos))
		{
			Position = targetPos;
            Position = ArenaGroup.PushBackInside(Position, _checkPoints.ToArray(), 1.0F);
		}
		else
		{
			Position = targetPos;
		}
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
			if (GetViewport().GetCamera2D() is BattleCamera camera)
				camera.StartShake(0.1f, Vector2.One, new Vector2(30, 30));
			UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("HURT"));
		}
	}

	public void SetInvincibleTimer(double timer = 0.0)
	{
		_invincibleTimer = timer;
    }
}
