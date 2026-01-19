using Godot;
using System;
using System.Collections.Generic;

public partial class EncounterChoiceMenu : BaseEncounterMenu
{

	[Export]
	public Godot.Collections.Array<EncounterChoiceMenuItem> MenuItems;
    public partial class ChoiceItem : RefCounted
	{
		public object ItemId;
		public string ItemDisplayName;
		public float MaxValue = 1;
		public float Value = 1;
		public ChoiceItem(object id)
		{
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

	protected int _currentChoice = 0; // 当前选择
	protected List<ChoiceItem> _items = [];

	public override void UIVisible() { }
	public override void UIHidden() { }

	public virtual void AddItem(object itemId, string displayName, float value = 1, float maxValue = 1)
	{
		_items.Add(new ChoiceItem(itemId).SetDisplayName(displayName).SetValue(value).SetMaxValue(maxValue));
	}
	public virtual void RemoveItem(int i)
	{
		if (i > -1 && i < _items.Count)
		{
			_items.RemoveAt(i);
		}
	}

	public object GetChoicedItemId()
	{
		return _items[_currentChoice].ItemId;
    }
    public string GetChoicedDisplayName()
    {
        return _items[_currentChoice].ItemDisplayName;
    }


    public void ClearItem()
	{
		_items.Clear();
	}

	public int GetItemCount()
	{
		return _items.Count;
	}

	public virtual void SetChoice(int Choice)
	{
		if (Choice >= _items.Count)
		{
			return;
		}
		_currentChoice = Choice;
	}
}
