using Godot;
using System;
using System.Threading.Tasks;

public partial class EncounterChoiceEnemyMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		ClearItem();
		for (int i = 0; i < GlobalBattleManager.Instance.GetEnemysCount(); i++)
		{
			BaseEnemy enemy = GlobalBattleManager.Instance.EnemysList[i];
			string enemyDiaplayText = ((enemy.CanSpare && enemy.AllowSpare) ? "[blend=yellow]" : "") +
				enemy.DisplayName;
			AddItem(i, enemyDiaplayText, enemy.MaxHp, enemy.Hp);
		}
	}
	public override void UIHidden()
	{
		
	}
}
