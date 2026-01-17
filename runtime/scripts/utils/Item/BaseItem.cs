using Godot;
using System;

[GlobalClass] // Godot推荐为自定义Resource添加该特性
public partial class BaseItem : Resource
{
    [Export]
    public string DisplayName { get; set; } = "ITEM";

    [Export]
    public int Slot { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string UseMessage { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string DropMessage { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string InfoMessage { get; set; } = string.Empty;

    public virtual void Use()
    {

    }

    public virtual void Drop()
    {

    }

    public virtual void ShowInfo()
    {

    }
}