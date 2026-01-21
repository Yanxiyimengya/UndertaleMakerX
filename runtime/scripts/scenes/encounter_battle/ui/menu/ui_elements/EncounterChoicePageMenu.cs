using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class EncounterChoicePageMenu : EncounterChoiceMenu
{
	[Export]
	public TextTyper PageTextTyper;

	public override void UIVisible() { }
	public override void UIHidden() { }

	public void HpBarSetVisible(bool v)
	{
		foreach (EncounterChoiceMenuItem item in MenuItems)
		{
			item.ProgressVisible = v;
		}
	}

	public async override void SetChoice(int choice)
	{
		if (choice >= _items.Count)
		{
			return;
		}
		int page = choice / MenuItems.Count;
		_currentChoice = choice;
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			if (_items.Count > MenuItems.Count)
			{
				PageTextTyper.Start($"PAGE {page + 1}");
				PageTextTyper.Visible = true;
			}
			else
			{
				PageTextTyper.Visible = false;
			}
			
			for (var i = 0; i < MenuItems.Count; i++)
			{
				int slot = (page * MenuItems.Count) + i;
				EncounterChoiceMenuItem menuItem = MenuItems[i];
				if (slot >= _items.Count)
				{
					menuItem.Visible = false;
					continue;
				}
				ChoiceItem choiceItem = _items[slot];
				menuItem.Visible = true;
				menuItem.Text = choiceItem.ItemDisplayName;
				if (menuItem.ProgressVisible)
				{
					menuItem.ProgressMaxValue = choiceItem.MaxValue;
					menuItem.ProgressValue = choiceItem.Value;
				}
			}

			
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            int soulSelectIndex = choice - (page * MenuItems.Count);
            if (soulSelectIndex < 3)
            {
				var menuItem = MenuItems[soulSelectIndex];
                BattlePlayerSoul soul = enc.GetPlayerSoul();
                soul.GlobalTransform = menuItem.GetSoulTransform();
            }
        }
	}
}
