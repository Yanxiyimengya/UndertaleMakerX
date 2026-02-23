using Godot;
using Godot.Collections;
[GlobalClass]
public partial class BaseEncounter : Resource
{
    [Export(PropertyHint.MultilineText)]
    public string EncounterText = "";
    [Export(PropertyHint.MultilineText)]
    public string FreeText = "";
    [Export(PropertyHint.MultilineText)]
    public string DeathText = "";
    [Export(PropertyHint.MultilineText)]
    public string EndText = "";

    [Export]
    public UtmxBattleManager.BattleStatus EncounterBattleFirstState = UtmxBattleManager.BattleStatus.Player;
    [Export]
    public string[] Enemies = ["MyEnemy"];
    [Export]
    public bool CanFree = true;

    public virtual void _OnBattleStart()
    {
    }
    public virtual void _OnGameover()
    {
    }
    public virtual void _OnBattleEnd()
    {
    }
    public virtual void _OnPlayerTurn()
    {
    }
    public virtual void _OnPlayerDialogue()
    {
    }
    public virtual void _OnEnemyDialogue()
    {
    }
    public virtual void _OnEnemyTurn()
    {
    }
}
