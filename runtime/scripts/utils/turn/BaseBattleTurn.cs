using Godot;

public partial class BaseBattleTurn : RefCounted
{
	public Vector2 arenaInitSize = new Vector2(155, 130);
	public Vector2 SoulInitializePosition = Vector2.Zero;
	public double TurnTime = 0.5;
	public bool Ending = false;
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
