using Godot;
using System;

public partial class EncounterItemMenu : EncounterChoiceListMenu
{
	public override void UIVisible()
	{
		ClearItem();
		for (int i = 0; i < PlayerDataManager.Instance.Items.Count; i ++)
		{
			BaseItem item = PlayerDataManager.Instance.Items[i];
			AddItem(i, item.DisplayName, 1, 1);
		}
		HpBarSetVisible(false);
	}
	public override void UIHidden()
	{

	}
}
