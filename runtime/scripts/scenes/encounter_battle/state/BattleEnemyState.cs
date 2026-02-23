using Godot;
using System;

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
        soul.EnabledCollisionWithArena = true;
        started = UtmxBattleManager.GetBattleTurnController().TurnStart();
        foreach (BaseEnemy enemy in UtmxBattleManager.GetBattleEnemyController().EnemiesList)
        {
            enemy._OnTurnStarting();
        }
    }

    public override void _ExitState()
    {
        if (started)
        {
            UtmxBattleManager.GetBattleTurnController().TurnEnd();
        }
    }

    public override void _Process(double delta)
    {
        if (started && !UtmxBattleManager.GetBattleTurnController().IsTurnInProgress())
        {
            EndEnemyTurn();
            started = false;
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
        BattleMainArenaExpand _battleMainArena = UtmxBattleManager.GetBattleArenaController().MainArena;
        await _battleMainArena.Resize(new Vector2(565, 130), 0.4);
        UtmxBattleManager.GetBattleController().ChangeToPlayerTurnState();
        soul.Movable = false;
        soul.EnabledCollisionWithArena = false;
    }
}
