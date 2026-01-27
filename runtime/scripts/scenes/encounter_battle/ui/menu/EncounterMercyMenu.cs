using Godot;
using System;

public partial class EncounterMercyMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		ClearItem();
		bool allowSpare = false;
		bool canSpare = false;
		foreach (BaseEnemy enemy in GlobalBattleManager.Instance.EnemysList)
		{
			if (enemy.AllowSpare)
			{
				allowSpare |= enemy.AllowSpare;
				canSpare |= enemy.CanSpare;
			}
		}
		if (allowSpare)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
			AddItem("SPARE", $"{(canSpare ? "[blend=yellow]" : "")}Spare");
		if (GlobalBattleManager.Instance.CanFree) 
			AddItem("FREE", "Free");
		ScrollBarSetVisible(false);
	}
	public override void UIHidden()
	{

	}
}
