using Godot;
using System;
using System.Threading.Tasks;

public partial class EncounterActChoiceEnemyMenu : EncounterChoiceMenu
{
	public override void UIVisible()
	{
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			ClearItem();
			foreach (BaseEnemy enemy in enc.Enemys) {
				AddItem(enemy.DisplayName, enemy.MaxHp, enemy.Hp);
			}
		}
	}
	public override void UIHidden()
	{
		
	}
}
