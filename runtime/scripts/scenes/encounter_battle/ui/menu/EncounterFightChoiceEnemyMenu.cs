using Godot;
using System;
using System.Threading.Tasks;

public partial class EncounterFightChoiceEnemyMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			ClearItem();
			for (int i = 0; i < enc.Enemys.Count; i++)
			{
				BaseEnemy enemy = enc.Enemys[i];
				AddItem(i, enemy.DisplayName, enemy.MaxHp, enemy.Hp);
				//TEMP
			}
		}
		HpBarSetVisible(true);
	}
	public override void UIHidden()
	{
		
	}
}
