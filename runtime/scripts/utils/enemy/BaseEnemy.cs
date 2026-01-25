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
    public int EnemyIndex = 0;
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
        AddDialogue("你打我干什么啊啊啊");
        AddDialogue("我要揍你！！！");
    }

    protected void AddDialogue(string dialogueMessage ,Vector2? offset = null, bool hideSpike = false, int dir = 2)
    {
        if (offset == null) 
            offset = new Vector2(30, 0);
        DialogueQueueManager.Instance.AppendBattleEnemyDialogue(EnemyIndex, dialogueMessage, (Vector2)offset, hideSpike);
    }

}
