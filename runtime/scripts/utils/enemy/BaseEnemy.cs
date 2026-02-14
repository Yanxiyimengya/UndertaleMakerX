using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class BaseEnemy : GameSprite2D
{
	public string DisplayName { get; set; } = "ENEMY";
	public int EnemySlot { get; set; } = 0;
	public float Attack { get; set; } = 0.0F;
	public float Defence { get; set; } = 0.0F;
	public float Hp { get; set; } = 10.0F;
	public float MaxHp { get; set; } = 10.0F;
	public bool AllowSpare { get; set; } = true;
	public bool CanSpare { get; set; } = false;
	public string MissText { get; set; } = "MISS";
	public string[] Actions { get; set; } = ["CHECK"];
	public Vector2 CenterPosition { get; set; } = new Vector2(0.0F, -80.0F);
	public virtual void _OnDialogueStarting()
	{
	}
	public virtual void _OnTurnStarting()
	{
	}
	public virtual void _HandleAction(string action)
	{
	}
	public virtual void _HandleAttack(UtmxBattleManager.AttackStatus status)
	{
	}
	public virtual void _OnSpare()
	{
	}
	public virtual void _OnDead()
	{
	}

	public override void _Ready() => _UpdateOffset();
	public override void _Process(double delta) => _UpdateOffset();


	public void _UpdateOffset()
	{
		Texture2D texture = SpriteFrames?.GetFrameTexture(DEFAULT_ANIM_NAME, Frame);
		if (texture != null)
			Offset = new Vector2(0, -texture.GetSize().Y * 0.5F);
	}

	public virtual BaseBattleTurn _GetNextTurn()
	{
		return new BaseBattleTurn();
	}
	public void hurt(double damage)
	{
		Hp -= (float)damage;
		if (Hp <= 0)
		{
			kill();
			return;
		}
		Hp = Math.Clamp(Hp, 0, MaxHp);
	}
	public void kill()
	{
		UtmxBattleManager.GetBattleEnemyController().KillEnemy(EnemySlot);
	}
}
