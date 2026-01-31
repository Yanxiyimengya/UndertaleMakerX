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
	public BattleEnemyController EnemyController;
	[Export]
	public BattlePlayerController PlayerController;
	[Export]
	public BattleTurnController TurnController;
	[Export]
	public BattleProjectileController ProjectileController;

	private UtmxBattleManager.BattleStatus _battleStatus = UtmxBattleManager.BattleStatus.Player;

	public override void _Ready()
	{
		UtmxBattleManager.Instance.InitializeBattle(this);
		UtmxBattleManager.Instance.GetEncounterInstance()?._OnBattleStart();
		switch (UtmxBattleManager.Instance.GetEncounterInstance()?.EncounterBattleFirstState)
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
		}
	}

	public void ChangeToPlayerTurnState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.Player;
		SwitchToState("BattlePlayerChoiceActionState");
		UtmxBattleManager.Instance.GetEncounterInstance()?._OnPlayerTurn();
	}
	public void ChangeToPlayerDialogueState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.PlayerDialogue;
		SwitchToState("BattlePlayerDialogueState");
		UtmxBattleManager.Instance.GetEncounterInstance()?._OnPlayerDialogue();
	}
	public void ChangeToEnemyDialogueState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.EnemyDialogue;
		SwitchToState("BattleEnemyDialogueState");
		UtmxBattleManager.Instance.GetEncounterInstance()?._OnEnemyDialogue();
	}
	public void ChangeToEnemyTurnState()
	{
		_battleStatus = UtmxBattleManager.BattleStatus.Enemy;
		SwitchToState("BattleEnemyState");
		UtmxBattleManager.Instance.GetEncounterInstance()?._OnEnemyTurn();
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
