using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class BattlePlayerSoul : CharacterBody2D
{
    [Export]
    BattleArenaGroup ArenaGroup;
    [Export]
    public float CollisionRadius = 8F;
    [Export]
    public bool EnableCollision
    {
        get => _enableCollision;
        set
        {
            _enableCollision = value;
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
                animSprite2d.Play("free");
                EnableCollision = false;
                Movable = false;
            }
        }
    }
    [Export]
    public Color SoulColor
    {
        get => Modulate;
        set
        {
            Modulate = value;
        }
    }

    [Export]
    public AnimatedSprite2D animSprite2d;
    [Export]
    public AnimationPlayer animPlayer;

    private uint _collisionLayer = 0;
    private uint _collisionMask = 0;
    private bool _enableCollision = true;
    private bool _movable = true;
    private bool _freed = false;
    private double _invincibleTimer = 0.0F;
    private List<Vector2> _checkPoints = new List<Vector2>();

    public const float Speed = 145.0f;
    public const float JumpVelocity = -400.0f;

    public BattlePlayerSoul()
    {
        CollisionLayer = (int)GlobalBattleManager.BattleCollisionLayers.Player;
        CollisionMask = (int)GlobalBattleManager.BattleCollisionLayers.Player;
    }

    public override void _Ready()
    {
        int count = 8;
        for (float i = 0; i < count; i++)
        {
            _checkPoints.Add(CollisionRadius * Vector2.Down.Rotated((i / count) * Mathf.Tau));
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_invincibleTimer > 0)
        {
            _invincibleTimer -= delta;
            animPlayer.Play("hurt");
        }
        else
        {
            animPlayer.Play("RESET");
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
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down").Normalized();
        Vector2 targetPos = GlobalPosition + inputDir * Speed * (float)delta;
        GlobalPosition = targetPos;

        if (!IsInsideArena(targetPos))
        {
            while (!IsInsideArena(GlobalPosition))
            {
                GlobalPosition = ArenaGroup.PushBackInside(GlobalPosition, _checkPoints.ToArray(), 1.0F);
            }
        }
        MoveAndSlide();
    }

    private bool IsInsideArena(Vector2 center)
    {
        if (!_enableCollision) return true;
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

    public void Hurt(double damage)
    {
        if (_invincibleTimer > 0)
            return;
        PlayerDataManager.Instance.PlayerHp -= (float)damage;
        _invincibleTimer = PlayerDataManager.Instance.PlayerInvincibleTime;
        UtmxGlobalStreamPlayer.Instance.PlaySound(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("HURT"));
        if (PlayerDataManager.Instance.PlayerHp <= 0)
        {
            GlobalBattleManager.Instance.GameOver();
        }
    }
}
