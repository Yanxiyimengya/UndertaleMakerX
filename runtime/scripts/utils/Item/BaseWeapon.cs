using Godot;
using System;


[GlobalClass]
public partial class BaseWeapon : BaseItem
{
	[Export]
	public float Attack = 0.0F;
	[Export]
	public PackedScene AttackAnimation;

	public virtual float _CalculateDamage(float value, BaseEnemy targetEnemy)
	{
		float atk = PlayerDataManager.Instance.PlayerAttack + Attack;
		float def = targetEnemy.Defence;
		float damage = atk - def + GD.Randf()*2;
		if (value <= 12)
		{
			damage *= 2.2F;
		}
		else
		{
			damage *= (1 - value / 545) * 2F;
		}
		damage = Mathf.Round(damage);
		if (damage <= -1) {
			damage = -1;
		}
		return damage;
	}
}
