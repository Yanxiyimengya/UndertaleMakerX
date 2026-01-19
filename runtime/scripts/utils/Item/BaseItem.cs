using Godot;
using System;

[GlobalClass] // Godot推荐为自定义Resource添加该特性
public partial class BaseItem : Resource
{
    [Export]
    public string DisplayName { get; set; } = "ITEM";

    [Export]
    public int Slot { get; set; }

    public virtual void OnUseSelected()
    {
        DialogueQueueManager.Instance.AppendDialogue("ASNHIUDSHNUSAIDBUSHB");
    }

    public virtual void OnDropSelected()
    {

    }

    public virtual void OnInfoSelected()
    {

    }
}