using Godot;
using System;
using static Godot.HttpRequest;

[GlobalClass]
public partial class JavaScriptItemProxy : BaseItem
{
	public JavaScriptObjectInstance JsInstance;
	public override void _OnUseSelected()
	{
        object result = JsInstance.Invoke("onUsed", []);
        if (UtmxBattleManager.Instance.IsInBattle())
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
