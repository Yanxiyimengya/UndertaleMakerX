using Godot;
using Jint.Native;
using Jint.Native.Object;
using System;

[GlobalClass]
public partial class JavaScriptItemProxy : BaseItem, IJavaScriptObject
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
}
