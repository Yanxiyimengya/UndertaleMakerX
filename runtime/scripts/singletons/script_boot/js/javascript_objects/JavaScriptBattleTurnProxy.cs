using Godot;
using Jint.Native.Object;

public partial class JavaScriptBattleTurnProxy : BaseBattleTurn, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }
    public static IJavaScriptObject New(ObjectInstance objInstance)
    {
        JavaScriptBattleTurnProxy turn = new JavaScriptBattleTurnProxy();
        turn.JsInstance = objInstance;
        return turn;
    }

    public override void _OnTurnInit()
    {
        if (JsInstance?.HasProperty("onTurnInit") == true)
            ((IJavaScriptObject)this).Invoke("onTurnInit", []);
    }
    public override void _OnTurnStart()
    {
        if (JsInstance?.HasProperty("onTurnStart") == true)
            ((IJavaScriptObject)this).Invoke("onTurnStart", []);
    }
    public override void _OnTurnEnd()
    {
        if (JsInstance?.HasProperty("onTurnEnd") == true)
            ((IJavaScriptObject)this).Invoke("onTurnEnd", []);
    }
    public override void _OnTurnUpdate(double delta)
    {
        if (JsInstance?.HasProperty("onTurnUpdate") == true)
            ((IJavaScriptObject)this).Invoke("onTurnUpdate", [delta]);
    }
}
