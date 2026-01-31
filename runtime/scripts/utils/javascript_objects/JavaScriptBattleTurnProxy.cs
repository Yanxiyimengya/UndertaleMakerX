using Godot;

public partial class JavaScriptBattleTurnProxy : BaseBattleTurn, IJavaScriptObject
{
    public JavaScriptObjectInstance JsInstance { get; set; }
    public override void _OnTurnInitialize()
    {
        JsInstance?.Invoke("onTurnInitialize", []);
    }
    public override void _OnTurnStart()
    {
        JsInstance?.Invoke("onTurnStart", []);
    }
    public override void _OnTurnEnd()
    {
        JsInstance?.Invoke("onTurnEnd", []);
    }
    public override void _OnTurnUpdate(double delta)
    {
        GD.Print(JsInstance);
        JsInstance?.Invoke("onTurnUpdate", [delta]);
    }
}
