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
	
	private Queue<BaseBattleProjectile> projectilePool = new();

	public override void _ExitTree()
	{
		foreach (BaseBattleProjectile battleProjectile in projectilePool)
		{
			battleProjectile.QueueFree();
		}
	}

	public BaseBattleProjectile CreateProjectile(bool mask = false)
	{
		return CreateProjectile<BaseBattleProjectile>(mask);
	}
	public T CreateProjectile<T>(bool mask = false) where T : BaseBattleProjectile, new()
	{
		T projectile;
		if (projectilePool.Count > 0)
		{
			projectile = (T)projectilePool.Dequeue();
			projectile.Visible = true;
		}
		else { projectile = new T(); }
		AppendProjectile(projectile);
		return projectile;
	}

	public void AppendProjectile(BaseBattleProjectile projectile, bool mask = false)
	{
		projectile.ProcessMode = ProcessModeEnum.Inherit;
		projectile.SetPhysicsProcess(true);
		var targetParent = mask ? ArenaMask : ProjectilesNode;
		if (projectile.IsInsideTree()) { projectile.Reparent(ProjectilesNode); }
		else { ProjectilesNode.AddChild(projectile); }
	}

	public void DeleteProjectile(BaseBattleProjectile projectile)
	{
		projectile.ProcessMode = ProcessModeEnum.Disabled;
		projectile.SetPhysicsProcess(false);
		projectile.Visible = false;
		projectilePool.Enqueue(projectile);
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
