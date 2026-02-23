using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerDialogueState : StateNode
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
        BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
        soul.Visible = true;
        soul.EnabledCollisionWithProjectile = true;
    }

    private void NextStep()
    {
        BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
        soul.Visible = false;
        soul.EnabledCollisionWithProjectile = false;
        if (UtmxDialogueQueueManager.DialogueCount() > 0)
        {
            string dialogueText = UtmxDialogueQueueManager.GetNextDialogueAsText();
            TextMenu.ShowEncounterText(dialogueText);
        }
        else
        {
            if (UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount() == 0)
            {
                UtmxBattleManager.GetBattleController().ChangeToEndState();
            }
            else
            {
                UtmxBattleManager.GetBattleController().ChangeToEnemyDialogueState();
            }
        }
    }
}
