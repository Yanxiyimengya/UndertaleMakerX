using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EncounterBattle : Node
{
	[Export]
	public BattlePlayerSoul PlayerSoul;
	[Export]
	public BattleMainArenaExpand MainArena;
	[Export]
	StateMachine BattleStateMachine;
	[Export]
	Node2D EnemysNode;

	public override void _EnterTree()
	{
		foreach (BaseEnemy enemy in BattleManager.Instance.EnemysList)
		{
			EnemysNode.AddChild(enemy);
		}
	}

	public override void _Ready()
	{
		BattleManager.Instance.InitializeBattle(this, PlayerSoul, MainArena);
		BattleStateMachine.SwitchToState(BattleManager.Instance.Config.EncounterBattleFirstState);
		AddEnemy(new BaseEnemy(), new Vector2(0F, -0F));
		AddEnemy(new BaseEnemy(), new Vector2(0F, -0F));
		AddEnemy(new BaseEnemy(), new Vector2(0F, -0F));
		AddEnemy(new BaseEnemy(), new Vector2(0F, -0F));
	}
	public void AddEnemy(BaseEnemy enemy, Vector2 position)
	{
		if (!enemy.IsInsideTree())
		{
			EnemysNode.AddChild(enemy);
			enemy.Position = position;
		}
	}
}
