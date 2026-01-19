using Godot;
using System;

public partial class EncounterMercyMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			ClearItem();
			bool allowSpare = false;
			bool canSpare = false;
			foreach (BaseEnemy enemy in enc.Enemys)
			{
				if (enemy.AllowSpare)
				{
					allowSpare |= enemy.AllowSpare;
					canSpare |= enemy.CanSpare;
				}
			}
			if (allowSpare)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
				AddItem("SPARE", $"{(canSpare ? "[blend=yellow]" : "")}Spare");
			if (enc.CanFree) 
				AddItem("FREE", "Free");
			ScrollBarSetVisible(false);
		}
	}
	public override void UIHidden()
	{

	}
}
