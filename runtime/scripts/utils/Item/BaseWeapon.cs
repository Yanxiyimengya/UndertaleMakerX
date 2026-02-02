using Godot;

[GlobalClass]
public partial class BaseWeapon : BaseItem
{
	[Export]
	public float Attack = 0.0F;
	[Export]
	public PackedScene AttackAnimation;

	public virtual double _CalculateDamage(float value, BaseEnemy targetEnemy)
	{
		double atk = UtmxPlayerDataManager.PlayerAttack + Attack;
		double def = targetEnemy.Defence;
		double damage = atk - def + (GD.Randf() * 2);
		if (value <= 12)
		{
			damage *= 2.2F;
		}
		else
		{
			damage *= (1 - value / 545) * 2F;
		}
		damage = Mathf.Round(damage);
		if (damage <= 0)
		{
			damage = 0;
		}
		return damage;
	}
}
