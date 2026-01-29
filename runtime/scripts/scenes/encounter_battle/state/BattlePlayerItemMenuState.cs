using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerItemMenuState : StateNode
{
    [Export]
    BattleMenuManager MenuManager;
    [Export]
    EncounterItemMenu ItemChoiceMenu;

    public int ItemChoice = 0;

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("up"))
        {
            ItemChoice -= 1;
            if (ItemChoice < 0)
            {
                ItemChoice = 0;
            }
            else
            {
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }
            ItemChoiceMenu.SetChoice(ItemChoice);

        }
        else if (Input.IsActionJustPressed("down"))
        {
            ItemChoice += 1;
            if (ItemChoice >= UtmxPlayerDataManager.GetItemCount())
            {
                ItemChoice = UtmxPlayerDataManager.GetItemCount() - 1;
            }
            else
            {
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }
            ItemChoiceMenu.SetChoice(ItemChoice);
        }
        else if (Input.IsActionJustPressed("cancel"))
        {
            SwitchState("BattlePlayerChoiceActionState");
        }
        else if (Input.IsActionJustPressed("confirm"))
        {
            int itemChoiced = (int)ItemChoiceMenu.GetChoicedItemId();
            UtmxPlayerDataManager.UseItem(itemChoiced);
            SwitchState("BattlePlayerDialogState");
        }
    }

    public override async void _EnterState()
    {
        await MenuManager.OpenMenu("EncounterItemMenu");
        ItemChoice = Math.Clamp(ItemChoice, 0, ItemChoiceMenu.GetItemCount() - 1);
        ItemChoiceMenu.SetChoice(ItemChoice);
    }

    public override void _ExitState()
    {
    }

    public override bool _CanEnterState()
    {
        return UtmxPlayerDataManager.GetItemCount() > 0;
    }
}
