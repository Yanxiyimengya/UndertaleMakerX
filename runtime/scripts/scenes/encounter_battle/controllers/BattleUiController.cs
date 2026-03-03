using Godot;
using System;

public partial class BattleUiController : Node
{
    [Export]
    BattleScreenButtonManager ButtonManager;
    [Export]
    Control UiLayer;
    [Export]
    BattleStatusBar StatusBar;

    public bool UiVisible
    {
        get => UiLayer.Visible;
        set
        {
            UiLayer.Visible = value;
        }
    }

    public BattleScreenButton GetButtonById(string id)
    {
        ButtonManager.GetBattleButton(id, out BattleScreenButton result);
        return result;
    }
    public BattleScreenButton GetButtonByIndex(int index)
    {
        ButtonManager.TryGetBattleButtonId(index, out string buttonId);
        ButtonManager.GetBattleButton(buttonId, out BattleScreenButton result);
        return result;
    }

}
