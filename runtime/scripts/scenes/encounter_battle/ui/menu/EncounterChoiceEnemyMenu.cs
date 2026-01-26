using Godot;
using System;
using System.Threading.Tasks;

public partial class EncounterChoiceEnemyMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			ClearItem();
			for (int i = 0; i < BattleManager.Instance.GetEnemysCount(); i++)
			{
				BaseEnemy enemy = BattleManager.Instance.EnemysList[i];
				string enemyDiaplayText = ((enemy.CanSpare && enemy.AllowSpare) ? "[blend=yellow]" : "") +
					enemy.DisplayName;
				AddItem(i, enemyDiaplayText, enemy.MaxHp, enemy.Hp);
			}
		}
	}
	public override void UIHidden()
	{
		
	}
}
