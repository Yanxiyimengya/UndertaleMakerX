using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class BaseEnemy : Node2D
{
	public string DisplayName = "ENEMY";

	public float Attack = 0.0F;
	public float Defence = 0.0F;
	public float Hp = 100.0F;
	public float MaxHp = 100.0F;
    public int Slot = 0;
    public bool AllowSpare = true;
	public bool CanSpare = true;
    public string MissText = "MISS";
    public Vector2 CenterPosition = new Vector2(0.0F, -80.0F);

    public Array<string> Actions = ["CHECK"];

    public override void _Ready()
    {
        base._Ready();
    }

    public virtual void OnSpare()
    {
    }

    public virtual void HandleAction(string action)
    {
    }
    public virtual void HandleAttack(bool missed)
    {
    }

}
