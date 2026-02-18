using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System;

[GlobalClass]
public partial class JavaScriptWeaponProxy : BaseWeapon, IJavaScriptObject
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

    public override double onAttack(float value, BaseEnemy targetEnemy)
    {
        if (JsInstance.HasProperty("onAttack"))
        {
            JsValue result = ((IJavaScriptObject)this).Invoke("onAttack", [value, targetEnemy]);
            return result.AsNumber();
        }
        else
        {
            return base.onAttack(value, targetEnemy);
        }
    }
}
