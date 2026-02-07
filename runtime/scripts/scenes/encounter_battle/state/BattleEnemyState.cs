using Godot;
using System;
using System.Threading.Tasks;
using static Godot.WebSocketPeer;

[GlobalClass]
public partial class BattleEnemyState : StateNode
{
	private BaseBattleTurn turn;
	private bool started;
	private Tween _tween;
	private BattleRectangleArenaExpand _battleMainArena;

	public override void _EnterState()
	{
		_battleMainArena = UtmxBattleManager.GetBattleArenaController().MainArena;
		BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
		soul.Movable = true;
		started = UtmxBattleManager.GetBattleTurnController().TurnStart();
	}

	public override void _ExitState() { }

	public override void _Process(double delta)
	{
        if (UtmxBattleManager.IsInBattle())
        {
			if (started && !UtmxBattleManager.GetBattleTurnController().IsTurnInProgress())
			{
				EndEnemyTurn();
				started = false;
			}
        }
	}

	private async void EndEnemyTurn()
	{
		started = false;
        UtmxBattleManager.GetBattleProjectileController().DestroyProjectilesOnTurnEnd();
        BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
		soul.Visible = false;
		soul.Movable = false;
		if (_tween != null && _tween.IsRunning())
		{
			_tween.Kill();
		}
        BattleMainArenaExpand _battleMainArena = UtmxBattleManager.GetBattleArenaController().MainArena;
        await _battleMainArena.Resize(new Vector2(565, 130), 0.4);
        UtmxBattleManager.GetBattleController().ChangeToPlayerTurnState();
		soul.Movable = false;
	}
}
