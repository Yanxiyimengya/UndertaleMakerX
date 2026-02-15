using Godot;
using Jint.Native.Object;
public partial class JavaScriptEncounterProxy : BaseEncounter, IJavaScriptObject
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }
	public override void _OnBattleStart()
	{
		((IJavaScriptObject)this).Invoke("onBattleStart", []);
	}
	public override void _OnGameover()
	{
		((IJavaScriptObject)this).Invoke("onGameover", []);
	}
	public override void _OnBattleEnd()
	{
		((IJavaScriptObject)this).Invoke("onBattleEnd", []);
	}
	public override void _OnPlayerTurn()
	{
		((IJavaScriptObject)this).Invoke("onPlayerTurn", []);
	}
	public override void _OnPlayerDialogue()
	{
		((IJavaScriptObject)this).Invoke("onPlayerDialogue", []);
	}
	public override void _OnEnemyDialogue()
	{
		((IJavaScriptObject)this).Invoke("onEnemyDialogue", []);
	}
	public override void _OnEnemyTurn()
	{
		((IJavaScriptObject)this).Invoke("onEnemyTurn", []);
	}
}
