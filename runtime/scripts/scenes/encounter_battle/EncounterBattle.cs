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
	public StateMachine BattleStateMachine;
	[Export]
	public Node2D EnemysNode;

	public override void _ExitTree()
	{
		BattleManager.Instance.UninitializeBattle();
	}

	public override void _Ready()
	{
		for (int i = 0; i < BattleManager.Instance.GetEnemysCount(); i++)
		{
			BaseEnemy enemy = BattleManager.Instance.EnemysList[i];
			enemy.EnemyIndex = i;
			if (!enemy.IsInsideTree())
				EnemysNode.AddChild(enemy);
			else
				enemy.Reparent(EnemysNode);
		}
		BattleManager.Instance.InitializeBattle(this, PlayerSoul, MainArena);
		BattleStateMachine.SwitchToState(BattleManager.Instance.Config.EncounterBattleFirstState);
	}
}
