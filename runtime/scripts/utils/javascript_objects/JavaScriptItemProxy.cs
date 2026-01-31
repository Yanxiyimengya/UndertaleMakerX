using Godot;
using System;

[GlobalClass]
public partial class JavaScriptItemProxy : BaseItem, IJavaScriptObject
{
    public JavaScriptObjectInstance JsInstance { get; set; }
    public override void _OnUseSelected()
	{
		object result = JsInstance.Invoke("onUsed", []);
		if (UtmxBattleManager.IsInBattle())
		{
			UtmxBattleManager.Instance.ShowDialogueText(result);
		}
	}

	public override void _OnDropSelected()
	{
		object result = JsInstance.Invoke("onDrop", []);
	}

	public override void _OnInfoSelected()
	{
		object result = JsInstance.Invoke("onInfo", []);
	}
}
