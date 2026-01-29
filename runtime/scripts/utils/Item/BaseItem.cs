using Godot;

[GlobalClass]
public partial class BaseItem : Resource
{
    [Export]
    public string DisplayName { get; set; } = "ITEM";

    [Export]
    public int Slot { get; set; }

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
        UtmxPlayerDataManager.RemoveItem(Slot);
    }
}