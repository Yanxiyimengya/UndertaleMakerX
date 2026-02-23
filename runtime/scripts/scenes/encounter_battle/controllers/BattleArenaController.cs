using Godot;
using System;

public partial class BattleArenaController : Node
{
    [Export]
    public BattleArenaGroup ArenaGroup;
    [Export]
    public BattleMainArenaExpand MainArena;


    // 矩形竞技场扩展创建函数
    public BattleRectangleArenaExpand CreateRectangleArenaExpand()
    {
        BattleRectangleArenaExpand arena = new BattleRectangleArenaExpand();
        ArenaGroup.AddChild(arena);
        return arena;
    }

    // 矩形竞技场裁剪创建函数
    public BattleRectangleArenaCulling CreateRectangleArenaCulling()
    {
        BattleRectangleArenaCulling arena = new BattleRectangleArenaCulling();
        ArenaGroup.AddChild(arena);
        return arena;
    }
    // 圆形竞技场扩展创建函数
    public BattleCircleArenaExpand CreateCircleArenaExpand()
    {
        BattleCircleArenaExpand arena = new BattleCircleArenaExpand();
        ArenaGroup.AddChild(arena);
        return arena;
    }
    // 圆形竞技场裁剪创建函数
    public BattleCircleArenaCulling CreateCircleArenaCulling()
    {
        BattleCircleArenaCulling arena = new BattleCircleArenaCulling();
        ArenaGroup.AddChild(arena);
        return arena;
    }


    // 多边形竞技场扩展创建函数
    public BattlePolygonArenaExpand CreatePolygonArenaExpand()
    {
        BattlePolygonArenaExpand arena = new BattlePolygonArenaExpand();
        ArenaGroup.AddChild(arena);
        return arena;
    }
    // 多边形竞技场裁剪创建函数
    public BattlePolygonArenaCulling CreatePolygonArenaCulling()
    {
        BattlePolygonArenaCulling arena = new BattlePolygonArenaCulling();
        ArenaGroup.AddChild(arena);
        return arena;
    }

}
