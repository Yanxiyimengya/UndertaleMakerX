using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class JavaScriptEncounterProxy : BaseEncounter
{
    public JavaScriptObjectInstance JsInstance;

    public override void _OnBattleStart()
    {
        JsInstance?.Invoke("onBattleStart", []);
    }
    public override void _OnBattleEnd()
    {
        JsInstance?.Invoke("onBattleEnd", []);
    }
    public override void _OnPlayerTurn()
    {
        JsInstance?.Invoke("onPlayerTurn", []);
    }
    public override void _OnPlayerDialogue()
    {
        JsInstance?.Invoke("onPlayerDialogue", []);
    }
    public override void _OnEnemyDialogue()
    {
        JsInstance?.Invoke("onnEnemyDialogue", []);
    }
    public override void _OnEnemyTurn()
    {
        JsInstance?.Invoke("onnEnemyTurn", []);
    }
}
