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
	public bool AllowSpare = true;
	public bool CanSpare = true;
    public string MissText = "MISS";

    public Array<string> Actions = ["CHECK"];
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
