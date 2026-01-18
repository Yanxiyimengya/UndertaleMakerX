using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class EncounterChoiceListMenu : BaseEncounterMenu
{
	public partial class ChoiceItem : RefCounted
	{
		public object ItemId;
		public string ItemDisplayName;
		public float MaxValue = 1;
		public float Value = 1;
		public ChoiceItem(object id) {
			ItemId = id;
		}

		public ChoiceItem SetValue(float v)
		{
			Value = v;
			return this;
		}
		public ChoiceItem SetMaxValue(float v)
		{
			MaxValue = v;
			return this;
		}
		public ChoiceItem SetDisplayName(string displayName)
		{
			ItemDisplayName = displayName;
			return this;
		}
	}
	
	[Export]
	public UndertaleStyleScrollBar UtScrollBar;
	[Export]
	public Godot.Collections.Array<EncounterChoiceMenuItem> MenuItem = [null, null, null];

	private int _firstIndex = 0; // 菜单滑动窗口
	private int _currentChoice = 0; // 当前选择
	private List<ChoiceItem> _items= [];

	public override void UIVisible() { }
	public override void UIHidden() { }

	public void AddItem(object itemId, string displayName, float value = 1,float maxValue = 1)
	{ 
		_items.Add(new ChoiceItem(itemId).SetDisplayName(displayName).SetValue(value).SetMaxValue(maxValue));
		UtScrollBar.Count = _items.Count;
	}
	public void RemoveItem(int i)
	{
		if (i > -1 && i < _items.Count)
		{
			_items.RemoveAt(i);
			UtScrollBar.Count = _items.Count;
		}
	}

	public object GetChoicedItemId()
	{
		return _items[_currentChoice].ItemId;
	}

	public void ClearItem()
	{
		_items.Clear();
		UtScrollBar.Count = 0;
	}

	public void HpBarSetVisible(bool v)
	{
		foreach (EncounterChoiceMenuItem item in MenuItem)
		{
			item.ProgressVisible = v;
		}
	}
	public void ScrollBarSetVisible(bool v)
	{
		UtScrollBar.Visible = v;
	}

	public void SetChoice(int Choice)
	{ 
		if (Choice >= MenuItem.Count)
		{
			return;
		}
		_firstIndex = Math.Max(_firstIndex, Choice - 2);
		_firstIndex = Math.Min(Choice, _firstIndex);
		UtScrollBar.CurrentIndex = Choice;
		_currentChoice = Choice;
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			for (var i = 0; i < 3; i++)
			{
				int slot = (i + _firstIndex);
				EncounterChoiceMenuItem emi = MenuItem[i];
				if (slot >= _items.Count)
				{
					emi.Visible = false;
					continue;
				}

				ChoiceItem choiceItem = _items[slot];
				emi.Visible = true;
				emi.Text = choiceItem.ItemDisplayName;
				if (emi.ProgressVisible)
				{
					emi.ProgressMaxValue = choiceItem.MaxValue;
					emi.ProgressValue = choiceItem.Value;
				}

				if (Choice - _firstIndex == i)
				{
					BattlePlayerSoul soul = enc.GetPlayerSoul();
					soul.GlobalTransform = emi.GetSoulTransform();
				}
			}
		}
	}
}
