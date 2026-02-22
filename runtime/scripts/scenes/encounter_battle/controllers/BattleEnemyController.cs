using Godot;
using System;
using System.Collections.Generic;

public partial class BattleEnemyController : Node
{
	[Export]
	public Node2D EnemiesNode;
	[Export]
	public BattleMainArenaExpand MainArena;
	public List<BaseEnemy> EnemiesList { get => _enemiesList; set => _enemiesList = value; }

	private List<BaseEnemy> _enemiesList = [];

	public override void _EnterTree()
	{
		foreach (BaseEnemy enemy in _enemiesList) enemy.Free();
		_enemiesList.Clear();
		foreach (string enemyId in UtmxBattleManager.GetEncounterInstance().Enemies)
		{
			if (UtmxGameRegisterDB.TryGetEnemy(enemyId, out BaseEnemy enemy))
				_enemiesList.Add(enemy);
		}
		for (int i = 0; i < _enemiesList.Count; i++)
		{
			BaseEnemy enemy = _enemiesList[i];
			enemy.EnemySlot = i;
			if (enemy.IsInsideTree())
			{
				enemy.Reparent(EnemiesNode);
			}
			else
			{
				EnemiesNode.AddChild(enemy);
			}
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		EnemiesNode.GlobalPosition = new Vector2(320F, MainArena.GlobalPosition.Y - MainArena.Size.Y);
	}
	public int GetEnemiesCount()
	{
		return EnemiesList.Count;
	}
	public BaseEnemy GetEnemy(int slot)
	{
		return EnemiesList[slot];
	}

	public void KillEnemy(int slot)
	{
		BaseEnemy enemy = EnemiesList[slot];
		if (IsInstanceValid(enemy))
		{
			enemy._OnDead();
			_enemiesList.Remove(enemy);
			enemy.QueueFree();
		}
	}

	public void TriggerEnemiesUsedItemCallback()
	{
		BaseEnemy[] enemiesSnapshot = _enemiesList.ToArray();
		foreach (BaseEnemy enemy in enemiesSnapshot)
		{
			if (IsInstanceValid(enemy))
			{
				enemy._UsedItem();
			}
		}
	}
}
