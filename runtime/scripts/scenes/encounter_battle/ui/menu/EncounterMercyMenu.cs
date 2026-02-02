using Godot;
using System;

public partial class EncounterMercyMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		ClearDisplayItem();
		bool allowSpare = false;
		bool canSpare = false;
		foreach (BaseEnemy enemy in UtmxBattleManager.GetBattleEnemyController().EnemiesList)
		{
			if (enemy.AllowSpare)
			{
				allowSpare |= enemy.AllowSpare;
				canSpare |= enemy.CanSpare;
			}
		}
		if (allowSpare)
			AddDisplayItem("SPARE", $"{(canSpare ? "[blend=yellow]" : "")}Spare");
		if ((bool)UtmxBattleManager.GetEncounterInstance()?.CanFree)
			AddDisplayItem("FREE", "Free");
		ScrollBarSetVisible(false);
	}
	public override void UIHidden()
	{

	}
}
