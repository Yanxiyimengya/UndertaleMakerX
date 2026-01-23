using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class EncounterChoiceListMenu : EncounterChoiceMenu
{
	[Export]
	public UndertaleStyleScrollBar UtScrollBar;

	private int _firstIndex = 0; // 菜单滑动窗口

	public void HpBarSetVisible(bool v)
	{
		foreach (EncounterChoiceMenuItem item in MenuItems)
		{
			item.ProgressVisible = v;
		}
	}
	public void ScrollBarSetVisible(bool v)
	{
		UtScrollBar.Visible = v;
	}

	public async override void SetChoice(int choice)
	{
		if (choice >= GetItemCount())
		{
			return;
		}
		_firstIndex = Math.Max(_firstIndex, choice - 2);
		_firstIndex = Math.Min(choice, _firstIndex);
		UtScrollBar.Count = GetItemCount();
		UtScrollBar.CurrentIndex = choice;
		_currentChoice = choice;
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			for (var i = 0; i < 3; i++)
			{
				int slot = (i + _firstIndex);
				EncounterChoiceMenuItem menuItem = MenuItems[i];
				if (slot >= GetItemCount())
				{
					menuItem.Visible = false;
					continue;
				}

				ChoiceItem choiceItem = GetItem(slot);
				menuItem.Visible = true;
				menuItem.Text = choiceItem.ItemDisplayName;
				if (menuItem.ProgressVisible)
				{
					menuItem.ProgressMaxValue = choiceItem.MaxValue;
					menuItem.ProgressValue = choiceItem.Value;
				}
			}

			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
			int soulSelectIndex = choice - _firstIndex;
			if (soulSelectIndex < 3)
			{
				var menuItem = MenuItems[soulSelectIndex];
				BattlePlayerSoul soul = enc.GetPlayerSoul();
				soul.GlobalTransform = menuItem.GetSoulTransform();

			}
		}
	}
}
