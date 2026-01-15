using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerSoul : CharacterBody2D
{

	[Export]
	public bool EnableCollision {
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

	public bool Movable
	{
		get => _movable;
		set
		{
			_movable = value;
		}

	}

	private uint _collisionLayer = 0;
	private uint _collisionMask = 0;
	private bool _enableCollision = true;
	private bool _movable = true;

	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	public override void _PhysicsProcess(double delta)
	{
		if (_movable)
		{
			Vector2 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				velocity += GetGravity() * (float)delta;
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			if (direction != Vector2.Zero)
			{
				velocity.X = direction.X * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			}

			Velocity = velocity;
			MoveAndSlide();
		}
	}
}
