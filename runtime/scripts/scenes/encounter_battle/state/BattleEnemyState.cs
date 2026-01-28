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
        turnTimer = 0.0;
        _battleMainArena = GlobalBattleManager.Instance.GetMainArena();
        turn = GlobalBattleManager.Instance.GetCurrentTurn();
        BattlePlayerSoul soul = GlobalBattleManager.Instance.GetPlayerSoul();
        soul.Movable = true;
        if (turn == null)
        {
            EndEnemyTurn();
        }
    }

    public override void _ExitState()
    {
        BattlePlayerSoul soul = GlobalBattleManager.Instance.GetPlayerSoul();
        turn.End();
        GlobalBattleManager.Instance.NextTurn();
        soul.Movable = true;
        soul.Visible = true;
    }

    public override void _Process(double delta)
    {
        turn.Update();
        turnTimer += delta;
        if (!turnEnded && turnTimer >= turn.TurnTime)
        {
            EndEnemyTurn();
        }
    }

    private async void EndEnemyTurn()
    {
        BattlePlayerSoul soul = GlobalBattleManager.Instance.GetPlayerSoul();
        soul.Visible = false;
        soul.Movable = false;
        turnEnded = true;
        if (_tween != null && _tween.IsRunning())
        {
            _tween.Kill();
        }
        _tween = GetTree().CreateTween();
        _tween.TweenProperty(_battleMainArena, "Size", new Vector2(565, 130), 0.4);
        await ToSignal(_tween, Tween.SignalName.Finished);
        SwitchState("BattlePlayerChoiceActionState");
    }
}
