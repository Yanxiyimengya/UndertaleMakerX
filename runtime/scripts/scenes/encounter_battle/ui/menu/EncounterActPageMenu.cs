using Godot;
using System;

public partial class EncounterActPageMenu : EncounterChoicePageMenu
{
    [Export]
    BattlePlayerActMenuState battlePlayerActMenuState;
    public override void UIVisible()
    {
        ClearDisplayItem();
        BaseEnemy enemy = UtmxBattleManager.GetBattleEnemyController().EnemiesList[battlePlayerActMenuState.EnemyChoice];
        foreach (string act in enemy.Actions)
        {
            AddDisplayItem(act, act);
        }
    }
    public override void UIHidden()
    {

    }
}
