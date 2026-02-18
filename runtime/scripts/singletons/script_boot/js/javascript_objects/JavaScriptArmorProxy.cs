using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System;

[GlobalClass]
public partial class JavaScriptArmorProxy : BaseArmor, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }
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

    public override double onDefend(double value)
    {
        if (JsInstance.HasProperty("onDefend"))
        {
            JsValue result = ((IJavaScriptObject)this).Invoke("onDefend", [value]);
            return result.AsNumber();
        }
        else
        {
            return base.onDefend(value);
        }
    }
}
