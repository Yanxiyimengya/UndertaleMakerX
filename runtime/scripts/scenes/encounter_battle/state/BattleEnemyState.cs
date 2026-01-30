using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class BattleEnemyState : StateNode
{
    private BattleTurn turn;
    private double turnTimer;
    private bool turnEnded;
    private Tween _tween;
    private BattleRectangleArenaExpand _battleMainArena;

    public override void _EnterState()
    {
        turnEnded = false;
        _battleMainArena = UtmxBattleManager.Instance.GetMainArena();
        BattlePlayerSoul soul = UtmxBattleManager.Instance.GetPlayerSoul();
        soul.Movable = true;
        if (!UtmxBattleManager.Instance.TurnStart())
        {
            EndEnemyTurn();
        }
    }

    public override void _ExitState()
    {
        BattlePlayerSoul soul = UtmxBattleManager.Instance.GetPlayerSoul();
        soul.Movable = true;
        soul.Visible = true;
    }

    public override void _Process(double delta)
    {
        if (!UtmxBattleManager.Instance.TurnUpdate(delta))
        {
            EndEnemyTurn();
        }
    }

    private async void EndEnemyTurn()
    {
        if (!turnEnded)
        {
            UtmxBattleManager.Instance.TurnEnd();
            BattlePlayerSoul soul = UtmxBattleManager.Instance.GetPlayerSoul();
            soul.Visible = false;
            soul.Movable = false;
            turnEnded = true;
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
            UtmxBattleManager.Instance.GetBattleController().ChangeToPlayerTurnState();
        }
    }
}
