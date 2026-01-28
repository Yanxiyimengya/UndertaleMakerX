using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BattlePlayerChoiceActionState : StateNode
{
    [Export]
    BattleScreenButtonManager BattleButtonManager;
    [Export]
    EncounterTextMenu TextMenu;
    [Export]
    BattleMenuManager MenuManager;

    private Vector2 _prevInputVector = Vector2.Zero;
    public override void _Process(double delta)
    {
        Vector2 inputVector = Input.GetVector("left", "right", "up", "down");
        if (inputVector != _prevInputVector)
        {
            _prevInputVector = inputVector;
            if (BattleButtonManager.MoveButton(inputVector))
            {
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }
        }

        if (Input.IsActionJustPressed("confirm"))
        {
            BattleButtonManager.PressBattleButton(BattleButtonManager.GetCurrentHoverdBattleButtonId());
            UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SELECT"));
        }

        BattlePlayerSoul soul = GlobalBattleManager.Instance.GetPlayerSoul();
        if (BattleButtonManager.GetBattleButton(BattleButtonManager.GetCurrentHoverdBattleButtonId(),
            out BattleScreenButton btn))
        {
            soul.GlobalTransform = btn.GetSoulTransform();
        }
    }
    public override async void _EnterState()
    {
        await MenuManager.OpenMenu("EncounterTextMenu");
        TextMenu.ShowEncounterText(GlobalBattleManager.Instance.EncounterText);
        BattlePlayerSoul soul = GlobalBattleManager.Instance.GetPlayerSoul();
        soul.Movable = false;
        soul.Show();

        string id = BattleButtonManager.GetCurrentHoverdBattleButtonId();
        if (string.IsNullOrEmpty(id))
        {
            if (BattleButtonManager.GetBattleButtonCount() > 0)
            {
                BattleButtonManager.GetBattleButtonId(0, out id);
                BattleButtonManager.SetButtonHover(id);
            }
        }
        else
        {
            BattleButtonManager.SetButtonHover(BattleButtonManager.GetCurrentHoverdBattleButtonId());
        }
    }

    public override void _ExitState()
    {
    }
}
