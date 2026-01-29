using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerDialogState : StateNode
{
    [Export]
    BattleMenuManager MenuManager;
    [Export]
    EncounterTextMenu TextMenu;
    [Export]
    BattleScreenButtonManager BattleButtonManager;
    public override void _Process(double delta)
    {
        if (TextMenu.IsTextTyperFinished())
        {
            if (Input.IsActionJustPressed("confirm"))
            {
                NextStep();
            }
        }
    }

    public override async void _EnterState()
    {
        NextStep();
        await MenuManager.OpenMenu("EncounterTextMenu");
        BattleButtonManager.ResetAllBattleButton();
    }

    public override void _ExitState()
    {
        UtmxBattleManager.Instance.GetPlayerSoul().Visible = true;
    }

    private void NextStep()
    {
        BattlePlayerSoul soul = UtmxBattleManager.Instance.GetPlayerSoul();
        soul.Visible = false;
        if (UtmxDialogueQueueManager.Instance.DialogueCount() > 0)
        {
            string dialogueText = UtmxDialogueQueueManager.Instance.GetNextDialogueAsText();
            TextMenu.ShowEncounterText($"* {dialogueText}");
        }
        else
        {
            if (!UtmxBattleManager.Instance.Endded)
                SwitchState("BattleEnemyDialogueState");
        }
    }
}
