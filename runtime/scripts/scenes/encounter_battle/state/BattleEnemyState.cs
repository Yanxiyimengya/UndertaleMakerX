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
		BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
		soul.Visible = false;
		soul.Movable = false;
		if (_tween != null && _tween.IsRunning())
		{
			_tween.Kill();
		}
		Vector2 targetSize = new Vector2(565, 130);
		if (_battleMainArena.Size != targetSize)
		{
			_tween = GetTree().CreateTween();
			_tween.TweenProperty(_battleMainArena, "Size", targetSize, 0.4);
			await ToSignal(_tween, Tween.SignalName.Finished);
		}
		UtmxBattleManager.GetBattleController().ChangeToPlayerTurnState();
		soul.Movable = false;
	}
}
