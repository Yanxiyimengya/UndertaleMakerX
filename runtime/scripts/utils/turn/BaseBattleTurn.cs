using Godot;

public partial class BaseBattleTurn : RefCounted
{
    public Vector2 ArenaInitializeSize = new Vector2(155, 130);
    public Vector2 SoulInitializePosition = new Vector2(320, 320);
    public double TurnTime = 0.5;
    public virtual void _OnTurnInit()
    {
    }
    public virtual void _OnTurnStart()
    {
    }
    public virtual void _OnTurnEnd()
    {
    }
    public virtual void _OnTurnUpdate(double delta)
    {
    }
}
