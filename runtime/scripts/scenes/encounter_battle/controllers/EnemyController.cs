using Godot;
using System;

public partial class EnemyController : Node
{
	[Export]
	public Node2D EnemysNode;

	public override void _Ready()
	{
		for (int i = 0; i < UtmxBattleManager.Instance.GetEnemysCount(); i++)
		{
			BaseEnemy enemy = UtmxBattleManager.Instance.EnemysList[i];
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
}
