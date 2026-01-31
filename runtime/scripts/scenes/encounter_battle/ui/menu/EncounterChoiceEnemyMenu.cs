using Godot;
using System;
using System.Threading.Tasks;

public partial class EncounterChoiceEnemyMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		ClearDisplayItem();
		
		var EnemyList = UtmxBattleManager.Instance.GetBattleEnemyController().EnemyList;
		for (int i = 0; i < EnemyList.Count; i++)
		{
			BaseEnemy enemy = EnemyList[i];
			string enemyDiaplayText = ((enemy.CanSpare && enemy.AllowSpare) ? "[blend=yellow]" : "") +
				enemy.DisplayName;
			AddDisplayItem(i, enemyDiaplayText, enemy.Hp, enemy.MaxHp);
		}
	}
	public override void UIHidden()
	{

	}
}
