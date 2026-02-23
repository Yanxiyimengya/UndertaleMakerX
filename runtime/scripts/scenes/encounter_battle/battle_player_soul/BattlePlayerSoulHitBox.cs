using Godot;

[GlobalClass]
public partial class BattlePlayerSoulHitBox : Area2D
{
    public BattlePlayerSoulHitBox()
    {
        CollisionLayer = (int)UtmxBattleManager.BattleCollisionLayers.Player;
        CollisionMask = (int)UtmxBattleManager.BattleCollisionLayers.Player;
    }
}
