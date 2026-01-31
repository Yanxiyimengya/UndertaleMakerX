using Godot;
using System;

public partial class EncounterItemMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		ClearDisplayItem();
		HpBarSetVisible(false);
		int index = 0;
		foreach (BaseItem inventoryItem in UtmxPlayerDataManager.Items)
		{
            AddDisplayItem(index, inventoryItem.DisplayName, 1, 1);
			index++;
		}
	}
	public override void UIHidden()
	{
        ClearDisplayItem();
	}
}
