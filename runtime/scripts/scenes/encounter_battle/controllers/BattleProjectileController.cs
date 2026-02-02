using Godot;
using System;
using System.Collections.Generic;

public partial class BattleProjectileController : Node
{
	[Export]
	public Node2D ProjectilesNode;
	[Export]
	public BattleArenaMask ArenaMask;
	
	private Queue<BaseBattleProjectile> projectilePool = new();
	
	public T CreateProjectile<T>(bool mask = false) where T : BaseBattleProjectile, new()
	{
		T projectile;
		if (projectilePool.Count > 0)
		{
			projectile = (T)projectilePool.Dequeue();
			projectile.ProcessMode = ProcessModeEnum.Inherit;
			projectile.SetPhysicsProcess(true);
		}
		else { projectile = new T(); }

		var targetParent = mask ? ArenaMask : ProjectilesNode;
		if (projectile.IsInsideTree()) { projectile.Reparent(targetParent); }
		else { targetParent.AddChild(projectile); }
		return projectile;
	}

	public void DeletaProjectile(BaseBattleProjectile projectile)
	{
		projectile.ProcessMode = ProcessModeEnum.Disabled;
		projectile.SetPhysicsProcess(false);
		projectile.Reparent(null);
        projectilePool.Enqueue(projectile);
	}
}
