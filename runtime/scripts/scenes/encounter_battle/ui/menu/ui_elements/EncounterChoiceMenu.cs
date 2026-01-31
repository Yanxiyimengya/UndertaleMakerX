using Godot;
using System.Collections.Generic;

public partial class EncounterChoiceMenu : BaseEncounterMenu
{

    [Export]
    public Godot.Collections.Array<EncounterChoiceMenuItem> MenuItems;

    public partial class ChoiceItem : GodotObject
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
    private List<ChoiceItem> _items = [];

    ~EncounterChoiceMenu()
    {
        ClearDisplayItem();
    }


    public override void UIVisible() { }
    public override void UIHidden() { }

    public virtual void AddDisplayItem(object itemId, string displayName, float value = 1, float maxValue = 1)
    {
        _items.Add(new ChoiceItem(itemId).SetDisplayName(displayName).SetValue(value).SetMaxValue(maxValue));
    }

    public void ClearDisplayItem()
    {
        foreach (ChoiceItem item in _items)
        {
            item.Free();
        }
        _items.Clear();
    }

    public virtual void RemoveDisplayItem(int i)
    {
        if (i > -1 && i < GetItemCount())
        {
            _items[i].Free();
            _items.RemoveAt(i);
        }
    }

    public ChoiceItem GetDisplayItem(int index)
    {
        return _items[index];
    }

    public object GetChoicedItemId()
    {
        return _items[_currentChoice].ItemId;
    }
    public string GetChoicedDisplayName()
    {
        return _items[_currentChoice].ItemDisplayName;
    }

    public int GetItemCount()
    {
        return _items.Count;
    }

    public virtual void SetChoice(int Choice)
    {
        if (Choice >= GetItemCount())
        {
            return;
        }
        _currentChoice = Choice;
    }
}
