using Godot;
using System;
using System.Threading.Tasks;

public partial class EncounterChoiceEnemyMenu : EncounterChoiceListMenu
{
    public override void UIVisible()
    {
        ClearDisplayItem();

        var EnemiesList = UtmxBattleManager.GetBattleEnemyController().EnemiesList;
        for (int i = 0; i < EnemiesList.Count; i++)
        {
            BaseEnemy enemy = EnemiesList[i];
            string enemyDisplayText = ((enemy.CanSpare && enemy.AllowSpare) ? "[blend=yellow]" : "") +
                enemy.DisplayName;
            AddDisplayItem(i, enemyDisplayText, enemy.Hp, enemy.MaxHp);
        }
    }
    public override void UIHidden()
    {

    }
}
