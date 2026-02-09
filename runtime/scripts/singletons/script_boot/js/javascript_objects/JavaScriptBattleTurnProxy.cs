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
		((IJavaScriptObject)this).Invoke("onTurnInit", []);
	}
	public override void _OnTurnStart()
	{
		((IJavaScriptObject)this).Invoke("onTurnStart", []);
	}
	public override void _OnTurnEnd()
	{
		((IJavaScriptObject)this).Invoke("onTurnEnd", []);
	}
	public override void _OnTurnUpdate(double delta)
	{
		((IJavaScriptObject)this).Invoke("onTurnUpdate", [delta]);
	}
}
