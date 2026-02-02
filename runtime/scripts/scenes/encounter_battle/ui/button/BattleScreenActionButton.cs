using Godot;
using System;
using static Godot.WebSocketPeer;

public partial class BattleScreenActionButton : BattleScreenButton
{
    [Export]
    public string TargetState;

    public override void _Ready()
    {
        base._Ready();
        Hover = false;
        Connect(SignalName.ButtonPressed, Callable.From(_OpenTargetMenu));
    }

    private void _OpenTargetMenu()
    {
        UtmxBattleManager.SwitchBattleState(TargetState);
    }
}
