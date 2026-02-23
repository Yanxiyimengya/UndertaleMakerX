using Godot;
using Jint.Native.Object;
public partial class JavaScriptEncounterProxy : BaseEncounter, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }
    public override void _OnBattleStart()
    {
        if (JsInstance.HasProperty("onBattleStart"))
            ((IJavaScriptObject)this).Invoke("onBattleStart", []);
    }
    public override void _OnGameover()
    {
        if (JsInstance.HasProperty("onGameover"))
            ((IJavaScriptObject)this).Invoke("onGameover", []);
    }
    public override void _OnBattleEnd()
    {
        if (JsInstance.HasProperty("onBattleEnd"))
            ((IJavaScriptObject)this).Invoke("onBattleEnd", []);
    }
    public override void _OnPlayerTurn()
    {
        if (JsInstance.HasProperty("onPlayerTurn"))
            ((IJavaScriptObject)this).Invoke("onPlayerTurn", []);
    }
    public override void _OnPlayerDialogue()
    {
        if (JsInstance.HasProperty("onPlayerDialogue"))
            ((IJavaScriptObject)this).Invoke("onPlayerDialogue", []);
    }
    public override void _OnEnemyDialogue()
    {
        if (JsInstance.HasProperty("onEnemyDialogue"))
            ((IJavaScriptObject)this).Invoke("onEnemyDialogue", []);
    }
    public override void _OnEnemyTurn()
    {
        if (JsInstance.HasProperty("onEnemyTurn"))
            ((IJavaScriptObject)this).Invoke("onEnemyTurn", []);
    }
}
