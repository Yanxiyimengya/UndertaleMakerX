using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class BattlePlayerActMenuState : StateNode
{
    [Export]
    public BattleMenuManager MenuManager;
    [Export]
    public EncounterChoiceEnemyMenu encounterChoiceEnemyMenu;
    [Export]
    public EncounterActPageMenu encounterActPageMenu;

    public int EnemyChoice = 0;
    public int ActChoice = 0;
    private bool _selected = false;
    private int _prevActChoice = 0;

    public override async void _Process(double delta)
    {
        if (_selected)
        {
            int menuItemCount = encounterActPageMenu.MenuItems.Count;
            int pageCount = encounterActPageMenu.GetItemCount() / menuItemCount; // 页面总数
            int page = ActChoice / menuItemCount; // 当前页面索引
            int index = ActChoice % menuItemCount; // 获取ActChoice在当前页面得索引
            int lineCount = (menuItemCount / 2);
            if (Input.IsActionJustPressed("up"))
            {
                if (index < 2)
                {
                    ActChoice += (lineCount - 1) * 2;
                }
                else
                {
                    ActChoice -= 2;
                }
            }
            else if (Input.IsActionJustPressed("down"))
            {
                if ((int)(index / 2) == (lineCount - 1))
                {
                    ActChoice -= (lineCount - 1) * 2;
                }
                else
                {
                    ActChoice += 2;
                }
            }
            if (Input.IsActionJustPressed("right"))
            {
                if (index % 2 == 0)
                {
                    ActChoice += 1;
                }
                else
                {
                    if (page != (pageCount - 1)) ActChoice += menuItemCount - 1;
                    else ActChoice = index - 1;
                }
            }
            else if (Input.IsActionJustPressed("left"))
            {
                if (index % 2 == 1)
                {
                    ActChoice -= 1;
                }
                else
                {
                    if (page != 0) ActChoice -= menuItemCount - 1;
                    else ActChoice = (pageCount - 1) * menuItemCount + index + 1;
                }
            }

            if (ActChoice != _prevActChoice)
            {
                ActChoice = Math.Clamp(ActChoice, 0, encounterActPageMenu.GetItemCount() - 1);
                encounterActPageMenu.SetChoice(ActChoice);
                _prevActChoice = ActChoice;
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }

            if (Input.IsActionJustPressed("cancel"))
            {
                await _OpenEnemyChoiceMenu();
            }
            else if (Input.IsActionJustPressed("confirm"))
            {
                _NextState();
            }

        }
        else
        {
            if (Input.IsActionJustPressed("up"))
            {
                EnemyChoice -= 1;
                if (EnemyChoice < 0)
                {
                    EnemyChoice = 0;
                }
                else
                {
                    UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
                }
                encounterChoiceEnemyMenu.SetChoice(EnemyChoice);
            }
            else if (Input.IsActionJustPressed("down"))
            {
                if (GlobalBattleManager.Instance.GetEnemysCount() > 0)
                {
                    EnemyChoice += 1;
                    if (EnemyChoice >= GlobalBattleManager.Instance.GetEnemysCount())
                    {
                        EnemyChoice = GlobalBattleManager.Instance.GetEnemysCount() - 1;
                    }
                    else
                    {
                        UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
                    }
                    encounterChoiceEnemyMenu.SetChoice(EnemyChoice);
                }
            }
            else if (Input.IsActionJustPressed("cancel"))
            {
                SwitchState("BattlePlayerChoiceActionState");
            }
            else if (Input.IsActionJustPressed("confirm"))
            {
                await _OpenActMenu();
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SELECT"));
            }
        }
    }

    private async Task _OpenActMenu()
    {
        if (GlobalBattleManager.Instance.EnemysList[EnemyChoice].Actions.Count > 0)
        {
            _selected = true;
            await MenuManager.OpenMenu("EncounterActPageMenu");
            encounterActPageMenu.SetChoice(ActChoice);
        }
    }

    private async Task _OpenEnemyChoiceMenu()
    {
        _selected = false;
        await MenuManager.OpenMenu("EncounterChoiceEnemyMenu");
        encounterChoiceEnemyMenu.HpBarSetVisible(false);
        encounterChoiceEnemyMenu.SetChoice(EnemyChoice);
    }

    private void _NextState()
    {
        string actionCommand = encounterActPageMenu.GetChoicedDisplayName();
        GlobalBattleManager.Instance.EnemysList[EnemyChoice].HandleAction(actionCommand);
        SwitchState("BattlePlayerDialogState");
    }

    public override async void _EnterState()
    {
        await _OpenEnemyChoiceMenu();
    }

    public override void _ExitState()
    {
    }
    public override bool _CanEnterState()
    {
        return GlobalBattleManager.Instance.GetEnemysCount() > 0;
    }
}
