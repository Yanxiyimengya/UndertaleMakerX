using Godot;
using System;
using System.Collections.Generic;

public partial class ProjectileController : Node
{
	[Export]
	public Node2D ProjectilesNode;
	[Export]
	public BattleArenaMask ArenaMask;
	
	private List<BaseBattleProjectile> projectileList = new();
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

		projectileList.Add(projectile);
		return projectile;
	}

	public void DeletaProjectile(BaseBattleProjectile projectile)
	{
		projectile.ProcessMode = ProcessModeEnum.Disabled;
		projectile.SetPhysicsProcess(false);
		projectileList.Remove(projectile);
		projectilePool.Enqueue(projectile);
	}
}
