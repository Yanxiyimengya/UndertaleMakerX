using Godot;
using Jint.Native.Object;
public partial class JavaScriptEncounterProxy : BaseEncounter, IJavaScriptObject
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }
	public override void _OnBattleStart()
	{ 
		if (JsInstance.HasOwnProperty("onBattleStart"))
			((IJavaScriptObject)this).Invoke("onBattleStart", []);
	}
	public override void _OnGameover()
    {
        if (JsInstance.HasOwnProperty("onGameover"))
            ((IJavaScriptObject)this).Invoke("onGameover", []);
	}
	public override void _OnBattleEnd()
    {
        if (JsInstance.HasOwnProperty("onBattleEnd"))
            ((IJavaScriptObject)this).Invoke("onBattleEnd", []);
	}
	public override void _OnPlayerTurn()
    {
        if (JsInstance.HasOwnProperty("onPlayerTurn"))
            ((IJavaScriptObject)this).Invoke("onPlayerTurn", []);
	}
	public override void _OnPlayerDialogue()
    {
        if (JsInstance.HasOwnProperty("onPlayerDialogue"))
            ((IJavaScriptObject)this).Invoke("onPlayerDialogue", []);
	}
	public override void _OnEnemyDialogue()
    {
        if (JsInstance.HasOwnProperty("onEnemyDialogue"))
            ((IJavaScriptObject)this).Invoke("onEnemyDialogue", []);
	}
	public override void _OnEnemyTurn()
    {
        if (JsInstance.HasOwnProperty("onEnemyTurn"))
            ((IJavaScriptObject)this).Invoke("onEnemyTurn", []);
	}
}
