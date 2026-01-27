using Godot;
using System;
using System.Collections.Generic;

public partial class BattleProjectileManager : Node
{
    List<BaseBattleProjectile> projectiles = new List<BaseBattleProjectile>();
    Queue<BaseBattleProjectile> projectilesPool = new Queue<BaseBattleProjectile>();

    public T SpawnProjectile<T>() where T : BaseBattleProjectile, new()
    {
        BaseBattleProjectile projectile;
        if (projectilesPool.Count > 0)
        {
            projectile = projectilesPool.Dequeue();
        }
        else
        {
            projectile = new T();
            AddChild(projectile);
        }
        projectiles.Add(projectile);
        projectile.Visible = true;
        return (T)projectile;
    }
}
