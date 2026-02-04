using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class BaseEnemy : Node2D
{
    public enum AttackStatus
    {
        Selected,  // 菜单中被选中
        Hit,     // 确认攻击
        Missed      // 未确认
    }

    public string DisplayName { get; set; } = "ENEMY";
    public int EnemySlot { get; set; } = 0;
    public float Attack { get; set; } = 0.0F;
    public float Defence { get; set; } = 0.0F;
    public float Hp { get; set; } = 100.0F;
    public float MaxHp { get; set; } = 100.0F;
    public bool AllowSpare { get; set; } = true;
    public bool CanSpare { get; set; } = false;
    public string MissText { get; set; } = "MISS";
    public string[] Actions { get; set; } = ["CHECK"];
    public Vector2 CenterPosition { get; set; } = new Vector2(0.0F, -80.0F);

    public virtual void _OnBattleStart()
    {
    }
    public virtual void _OnBattleEnd()
    {
    }
    public virtual void _OnDialogueStarting()
    {
    }
    public virtual void _OnDialogueEnding()
    {
    }
    public virtual void _HandleAction(string action)
    {
    }
    public virtual void _HandleAttack(AttackStatus status)
    {
    }
    public virtual void _OnSpare()
    {
    }

    public virtual BaseBattleTurn _GetNextTurn()
    {
        return new BaseBattleTurn();
    }
}
