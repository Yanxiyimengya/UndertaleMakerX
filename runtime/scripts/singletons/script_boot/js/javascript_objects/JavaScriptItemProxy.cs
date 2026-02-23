using Godot;
using Jint.Native;
using Jint.Native.Object;
using System;

[GlobalClass]
public partial class JavaScriptItemProxy : BaseItem, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }

    public string displayName
    {
        get => DisplayName;
        set => DisplayName = value;
    }
    public object[] usedText
    {
        get => UsedText;
        set { UsedText = Array.ConvertAll<object, string>(value, s => (string)s); }
    }
    public object[] infoText
    {
        get => InfoText;
        set { InfoText = Array.ConvertAll<object, string>(value, s => (string)s); }
    }
    public object[] dropText
    {
        get => DroppedText;
        set { DroppedText = Array.ConvertAll<object, string>(value, s => (string)s); }
    }
    public double getSlot() => ItemSlot;
    public void removeSelf() => RemoveSelf();

    public override void _OnUseSelected()
    {
        ((IJavaScriptObject)this).Invoke("onUse", []);
    }

    public override void _OnDropSelected()
    {
        ((IJavaScriptObject)this).Invoke("onDrop", []);
    }

    public override void _OnInfoSelected()
    {
        ((IJavaScriptObject)this).Invoke("onInfo", []);
    }
}
