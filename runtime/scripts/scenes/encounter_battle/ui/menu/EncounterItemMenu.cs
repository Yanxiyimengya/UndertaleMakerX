using Godot;
using System;

public partial class EncounterItemMenu : BaseEncounterMenu
{
	[Export]
	UndertaleStyleScrollBar UtScrollBar;
	[Export]
	Godot.Collections.Array<EncounterChoiceMenuItem> ItemMenuItem = [null, null, null];

	private int firstIndex = 0; // 菜单滑动窗口
	
	public override void UIVisible()
	{
		UtScrollBar.Count = PlayerDataManager.Instance.GetItemCount();
	}
	public override void UIHidden()
	{

	}
	
	public void SetChoice(int Choice)
	{
		firstIndex = Math.Max(firstIndex, Choice - 2);
		firstIndex = Math.Min(Choice, firstIndex);
		UtScrollBar.CurrentIndex = Choice;
		for (var i = 0; i < 3; i++)
		{
			if (GetTree().CurrentScene is EncounterBattle enc)
			{
				EncounterChoiceMenuItem emi = ItemMenuItem[i];
				int slot = (i + firstIndex);
				if (slot < PlayerDataManager.Instance.GetItemCount())
				{
					emi.Text = $"{PlayerDataManager.Instance.Items[slot].DisplayName}{slot}";
				}
				if (Choice - firstIndex == i)
				{
					BattlePlayerSoul soul = enc.GetPlayerSoul();
					soul.GlobalTransform = emi.GetSoulTransform();
				}
			}
		}
	}
}
