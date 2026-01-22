using Godot;
using System;

[GlobalClass]
public partial class BaseItem : Resource
{
    [Export]
    public string DisplayName { get; set; } = "ITEM";

    [Export]
    public int Slot { get; set; }

    public virtual void OnUseSelected()
    {
    }

    public virtual void OnDropSelected()
    {

    }

    public virtual void OnInfoSelected()
    {

    }
}