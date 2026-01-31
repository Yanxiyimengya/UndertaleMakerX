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

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("left") || Input.IsActionJustPressed("right"))
        {
            if (BattleButtonManager.MoveButton(new Vector2(Input.GetAxis("left", "right"), 0F)))
            {
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }
        } else if (Input.IsActionJustPressed("up") || Input.IsActionJustPressed("down"))
        {
            if (BattleButtonManager.MoveButton(new Vector2(0F, Input.GetAxis("up", "down"))))
            {
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }
        }

        if (Input.IsActionJustPressed("confirm"))
        {
            BattleButtonManager.PressBattleButton(BattleButtonManager.GetCurrentHoverdBattleButtonId());
            UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SELECT"));
        }

        BattlePlayerSoul soul = UtmxBattleManager.Instance.GetBattleController().PlayerSoul;
        if (BattleButtonManager.GetBattleButton(BattleButtonManager.GetCurrentHoverdBattleButtonId(),
            out BattleScreenButton btn))
        {
            soul.GlobalTransform = btn.GetSoulTransform();
        }
    }
    public override async void _EnterState()
    {
        await MenuManager.OpenMenu("EncounterTextMenu");
        TextMenu.ShowEncounterText(UtmxBattleManager.Instance.GetEncounterInstance()?.EncounterText);
        BattlePlayerSoul soul = UtmxBattleManager.Instance.GetBattleController().PlayerSoul;
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
