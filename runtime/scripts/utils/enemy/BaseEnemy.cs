using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class BaseEnemy : Node2D
{
    public string DisplayName = "ENEMY";
    public int EnemySlot = 0;

    public float Attack = 0.0F;
    public float Defence = 0.0F;
    public float Hp = 100.0F;
    public float MaxHp = 100.0F;
    public bool AllowSpare = true;
    public bool CanSpare = false;
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

    public void AppendDialogue(string dialogueMessage, Vector2? offset = null, bool hideSpike = false, int dir = 2)
    {
        if (offset == null)
            offset = new Vector2(30, 0);
        UtmxDialogueQueueManager.Instance.AppendBattleEnemyDialogue(EnemySlot, dialogueMessage, (Vector2)offset, hideSpike);
    }
}
