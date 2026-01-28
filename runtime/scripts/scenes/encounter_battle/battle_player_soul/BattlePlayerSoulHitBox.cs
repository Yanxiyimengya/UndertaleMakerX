using Godot;

[GlobalClass]
public partial class BattlePlayerSoulHitBox : Area2D
{
	public BattlePlayerSoulHitBox()
	{
		CollisionLayer = (int)GlobalBattleManager.BattleCollisionLayers.Player;
		CollisionMask = (int)GlobalBattleManager.BattleCollisionLayers.Player;
	}
}
