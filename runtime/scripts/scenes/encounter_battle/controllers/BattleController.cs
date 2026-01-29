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

	public override void _Ready()
	{
		UtmxBattleManager.Instance.InitializeBattle(BattleStateMachine, PlayerSoul, MainArena);
		BattleStateMachine.SwitchToState(UtmxBattleManager.Instance.GetFirstBattleState());
	}
	
	
	
}
