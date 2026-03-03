using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerMercyMenuState : StateNode
{
    [Export]
    BattleMenuManager MenuManager;
    [Export]
    EncounterTextMenu TextMenu;
    [Export]
    BattleScreenButtonManager BattleButtonManager;
    [Export]
    EncounterMercyMenu MercyChoiceMenu;

    public int MercyChoice = 0;

    private bool _freed = false;
    private BattlePlayerSoul _playerSoul;

    public override void _Process(double delta)
    {
        if (_freed)
        {
            if (_playerSoul != null)
            {
                _playerSoul.Position = new Vector2(_playerSoul.Position.X - (float)(delta * 120),
                    _playerSoul.Position.Y);
            }
            if (TextMenu.IsTextTyperFinished())
            {
                if (Input.IsActionJustPressed("confirm"))
                {
                    UtmxBattleManager.EndEncounterBattle();
                }
            }
        }
        else
        {
            int itemCount = MercyChoiceMenu.GetItemCount();
            if (itemCount <= 0)
            {
                if (Input.IsActionJustPressed("confirm") || Input.IsActionJustPressed("cancel"))
                {
                    SwitchState("BattlePlayerChoiceActionState");
                }
                return;
            }

            if (Input.IsActionJustPressed("up"))
            {
                MercyChoice -= 1;
                if (MercyChoice < 0)
                {
                    MercyChoice = 0;
                }
                else
                {
                    UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
                }
                MercyChoiceMenu.SetChoice(MercyChoice);
            }
            else if (Input.IsActionJustPressed("down"))
            {
                MercyChoice += 1;
                if (MercyChoice >= itemCount)
                {
                    MercyChoice = itemCount - 1;
                }
                else
                {
                    UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
                }

                MercyChoiceMenu.SetChoice(MercyChoice);
            }
            else if (Input.IsActionJustPressed("cancel"))
            {
                SwitchState("BattlePlayerChoiceActionState");
            }
            else if (Input.IsActionJustPressed("confirm"))
            {
                UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SELECT"));
                string selected = MercyChoiceMenu.GetselectedItemId() as string;
                if (string.IsNullOrEmpty(selected))
                {
                    return;
                }
                if (selected == "SPARE")
                {
                    foreach (BaseEnemy enemy in UtmxBattleManager.GetBattleEnemyController().EnemiesList)
                    {
                        if (enemy.AllowSpare && enemy.CanSpare)
                        {
                            enemy._OnSpare();
                        }
                    }
                    _NextState();
                }
                else if (selected == "FREE")
                {
                    _Free();
                }
            }
        }
    }

    public override async void _EnterState()
    {
        _freed = false;
        await MenuManager.OpenMenu("EncounterMercyMenu");
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        int itemCount = MercyChoiceMenu.GetItemCount();
        if (itemCount <= 0)
        {
            SwitchState("BattlePlayerChoiceActionState");
            return;
        }

        MercyChoice = Math.Clamp(MercyChoice, 0, itemCount - 1);
        MercyChoiceMenu.SetChoice(MercyChoice);
    }
    public override void _ExitState()
    {
    }

    private void _NextState()
    {
        UtmxBattleManager.GetBattleController().ChangeToPlayerDialogueState();
    }

    private async void _OpenTextMenu()
    {
        await MenuManager.OpenMenu("EncounterTextMenu");
    }

    private void _Free()
    {
        _freed = true;
        UtmxBattleManager.Endded = true;
        _playerSoul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
        _playerSoul.Freed = true;
        _playerSoul.Visible = true;
        _OpenTextMenu();
        TextMenu.ShowEncounterText(UtmxBattleManager.GetEncounterInstance()?.FreeText);
        BattleButtonManager.ResetAllBattleButton();
        UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("ESCAPED"));
    }
}
