using Godot;

[GlobalClass]
public partial class BaseWeapon : BaseItem
{
	[Export]
	public float Attack = 0.0F;
    [Export]
    public string[] AttackAnimation = [
        "built-in-resources/textures/attack_animation/spr_slice_o_0.png",
        "built-in-resources/textures/attack_animation/spr_slice_o_1.png",
        "built-in-resources/textures/attack_animation/spr_slice_o_2.png",
        "built-in-resources/textures/attack_animation/spr_slice_o_3.png",
        "built-in-resources/textures/attack_animation/spr_slice_o_4.png",
        "built-in-resources/textures/attack_animation/spr_slice_o_5.png",
        ];
    [Export]
    public double AttackAnimationSpeed = 1.0;
    [Export]
    public string AttackSound = "built-in-resources/sounds/sfx/laz.wav";

    public virtual double onAttack(float value, BaseEnemy targetEnemy)
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
