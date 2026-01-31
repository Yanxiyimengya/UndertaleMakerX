using Godot;
using System;
using System.Collections.Generic;

public partial class BattleEnemyController : Node
{
	[Export]
	public Node2D EnemysNode;
	public List<BaseEnemy> EnemyList { get => _enemyList; set => _enemyList = value; }

	private List<BaseEnemy> _enemyList = [];

	public override void _Ready()
	{
		foreach (BaseEnemy enemy in EnemyList) enemy.Free();
		EnemyList.Clear();

		foreach (string enemyId in UtmxBattleManager.Instance.GetEncounterInstance().Enemies)
		{
			if (GameRegisterDB.TryGetEnemy(enemyId, out BaseEnemy eenemy))
			{
				EnemyList.Add(eenemy);
			}
		}

		for (int i = 0; i < EnemyList.Count; i++)
		{
			BaseEnemy enemy = EnemyList[i];
			enemy.EnemySlot = i;
			if (enemy.IsInsideTree())
			{
				enemy.Reparent(EnemysNode);
			}
			else
			{
				EnemysNode.AddChild(enemy);
			}
		}
	}
	
	public int GetEnemiesCount()
	{
		return EnemyList.Count;
	}
}
