using Godot;

[GlobalClass]
public partial class JavaScriptProxyItem : BaseItem
{
    public JavaScriptObjectInstance JsInstance;
    public override void _OnUseSelected()
    {
        JsInstance.Invoke("onUsed", []);
    }

    public override void _OnDropSelected()
    {
        JsInstance.Invoke("onDrop", []);
    }

    public override void _OnInfoSelected()
    {
        JsInstance.Invoke("onInfo", []);
    }
}
