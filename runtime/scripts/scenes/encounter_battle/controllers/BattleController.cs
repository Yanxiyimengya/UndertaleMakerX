using Godot;

[GlobalClass]
public partial class BattleController : Node
{
	[Export]
	public StateMachine BattleStateMachine;
	[Export]
	public BattleCamera Camera;
	[Export]
	public BattleEnemyController EnemyController;
	[Export]
	public BattlePlayerController PlayerController;
	[Export]
	public BattleArenaController ArenaController;
	[Export]
	public BattleTurnController TurnController;
	[Export]
	public BattleUiController UiController;
	[Export]
	public BattleProjectileController ProjectileController;

	private UtmxBattleManager.BattleStatus _battleStatus = UtmxBattleManager.BattleStatus.Player;

	public override void _Ready()
	{
		UtmxBattleManager.InitializeBattle(this);
		SwitchStatus((UtmxBattleManager.BattleStatus)UtmxBattleManager.GetEncounterInstance()?.EncounterBattleFirstState);
	}

	public void SwitchStatus(UtmxBattleManager.BattleStatus status)
	{
		switch (status)
		{
			case UtmxBattleManager.BattleStatus.Player:
				{
					ChangeToPlayerTurnState();
					break;
				}
			case UtmxBattleManager.BattleStatus.PlayerDialogue:
				{
					ChangeToPlayerDialogueState();
					break;
				}
			case UtmxBattleManager.BattleStatus.EnemyDialogue:
				{
					ChangeToEnemyDialogueState();
					break;
				}
			case UtmxBattleManager.BattleStatus.Enemy:
				{
					ChangeToEnemyTurnState();
					break;
				}
			case UtmxBattleManager.BattleStatus.End:
				{
					ChangeToEndState();
					break;
				}
		}
	}
	public void ChangeToPlayerTurnState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.Player;
		SwitchToState("BattlePlayerChoiceActionState");
		UtmxBattleManager.GetEncounterInstance()?._OnPlayerTurn();
	}
	public void ChangeToPlayerDialogueState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.PlayerDialogue;
		SwitchToState("BattlePlayerDialogueState");
		UtmxBattleManager.GetEncounterInstance()?._OnPlayerDialogue();
	}
	public void ChangeToEnemyDialogueState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.EnemyDialogue;
		SwitchToState("BattleEnemyDialogueState");
		UtmxBattleManager.GetEncounterInstance()?._OnEnemyDialogue();
	}
	public void ChangeToEnemyTurnState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.Enemy;
		SwitchToState("BattleEnemyState");
		UtmxBattleManager.GetEncounterInstance()?._OnEnemyTurn();
	}
	public void ChangeToEndState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.End;
		SwitchToState("BattleEndState");
	}
	public UtmxBattleManager.BattleStatus GetCurrentStatus()
	{
		return _battleStatus;
	}
	public void SwitchToState(string targetState)
	{
		BattleStateMachine.SwitchToState(targetState);
	}
}
