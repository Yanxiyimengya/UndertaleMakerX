using Godot;
using System;

public partial class EncounterItemMenu : EncounterChoiceMenu
{
	public override void UIVisible()
	{
		ClearItem();
		foreach (BaseItem item in PlayerDataManager.Instance.Items)
		{
			AddItem(item.DisplayName, 1, 1);
		}
		HpBarSetVisible(false);
	}
	public override void UIHidden()
	{

	}
}
