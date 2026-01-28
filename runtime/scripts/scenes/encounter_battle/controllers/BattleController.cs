using Godot;

[GlobalClass]
public partial class BattleController : Node
{
	[Export]
	public BattlePlayerSoul PlayerSoul;
	[Export]
	public BattleMainArenaExpand MainArena;
	[Export]
	public StateMachine BattleStateMachine;
	[Export]
	public Node2D EnemysNode;

	public override void _Ready()
	{
		for (int i = 0; i < GlobalBattleManager.Instance.GetEnemysCount(); i++)
		{
			BaseEnemy enemy = GlobalBattleManager.Instance.EnemysList[i];
			enemy.EnemyIndex = i;
			if (!enemy.IsInsideTree())
				EnemysNode.AddChild(enemy);
			else
				enemy.Reparent(EnemysNode);
		}
		GlobalBattleManager.Instance.InitializeBattle(BattleStateMachine, PlayerSoul, MainArena);
		BattleStateMachine.SwitchToState(GlobalBattleManager.Instance.Config.EncounterBattleFirstState);
	}
}
