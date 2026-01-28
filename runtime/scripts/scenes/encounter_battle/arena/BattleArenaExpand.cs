using Godot;
using System;

[GlobalClass]
public abstract partial class BattleArenaExpand : BaseBattleArena
{
    [Export]
    public Color BorderColor = Colors.White;
    [Export]
    public Color ContentColor = Colors.Black;

    public abstract bool IsPointInArena(Vector2 point);
    public abstract Vector2 GetRecentPointInArena(Vector2 point);
    public abstract bool IsSegmentInArena(Vector2 from, Vector2 to);
}
