using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BattleProjectileController : Node
{
	[Export]
	public Node2D ProjectilesNode;
	[Export]
	public BattleArenaMask ArenaMask;
	
	private ObjectPool<BaseBattleProjectile> _pool = new();

	public BaseBattleProjectile CreateProjectile(bool mask = false)
	{
		return CreateProjectile<BaseBattleProjectile>(mask);
	}
	public T CreateProjectile<T>(bool mask = false) where T : BaseBattleProjectile, new()
	{
		T projectile = _pool.GetObject<T>();
		var targetParent = mask ? ArenaMask : ProjectilesNode;
		if (projectile.IsInsideTree())
		{ projectile.Reparent(targetParent); }
		else { targetParent.AddChild(projectile); }
		return projectile;
	}

	public void DeleteProjectile(BaseBattleProjectile projectile)
	{
		_pool.DisabledObject(projectile);
	}

	public void DestroyProjectilesOnTurnEnd()
	{
		List<BaseBattleProjectile> destroyList = new();
		foreach (Node child in ProjectilesNode.GetChildren())
		{
			if (child is BaseBattleProjectile projectile && projectile.DestroyOnTurnEnd)
			{
				destroyList.Add(projectile);
			}
		}
		foreach (Node child in ArenaMask.GetChildren())
		{
			if (child is BaseBattleProjectile projectile && projectile.DestroyOnTurnEnd)
			{
				destroyList.Add(projectile);
			}
		}
		foreach (BaseBattleProjectile projectile in destroyList)
		{
			DeleteProjectile(projectile);
		}
	}
}
