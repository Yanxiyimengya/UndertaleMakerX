using Godot;

[GlobalClass]
public partial class BaseItem : Resource
{
    [Export]
    public string DisplayName { get; set; } = "ITEM";
    [Export]
    public int ItemSlot { get; set; }
    public string[] UsedText = [];
    public string[] DroppedText = [];
    public string[] InfoText = [];

    public virtual void _OnUseSelected()
    {
    }

    public virtual void _OnDropSelected()
    {
    }

    public virtual void _OnInfoSelected()
    {
    }

    public void RemoveSelf()
    {
        UtmxPlayerDataManager.RemoveItem(ItemSlot);
    }
}