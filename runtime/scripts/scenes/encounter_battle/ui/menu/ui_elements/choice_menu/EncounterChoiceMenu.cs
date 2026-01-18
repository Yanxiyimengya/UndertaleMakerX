using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class EncounterChoiceMenu : BaseEncounterMenu
{
	private partial class ChoiceItem : RefCounted
	{
		public string ItemName;
		public float MaxValue;
		public float Value;
		public ChoiceItem(string name, float maxValue = 1, float value = 1) {
			ItemName = name;
			MaxValue = maxValue;
			Value = value;
		}
	}

	[Export]
	public bool ScrollBarVisible
	{
		get => UtScrollBar.Visible;
		set
		{
			if (UtScrollBar != null)
			{
				UtScrollBar.Visible = value;
			}
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
	public void AddItem(string name, float maxValue, float value)
	{ 
		_items.Add(new ChoiceItem(name, maxValue, value));
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

	public void SetChoice(int Choice)
	{
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
				emi.Text = choiceItem.ItemName;
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
