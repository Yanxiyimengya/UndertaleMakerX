using Godot;
using System;

[GlobalClass]
public partial class BattleEndState : StateNode
{
    [Export]
    BattleScreenButtonManager BattleButtonManager;
    [Export]
    EncounterTextMenu TextMenu;
    [Export]
    BattleMenuManager MenuManager;

    public override async void _EnterState()
    {
        await MenuManager.OpenMenu("EncounterTextMenu");
        BattleButtonManager.ResetAllBattleButton();
        TextMenu.ShowEncounterText(GlobalBattleManager.Instance.EndText);
    }

    public override void _ExitState()
    {
        GlobalBattleManager.Instance.GetPlayerSoul().Visible = true;
    }
}
