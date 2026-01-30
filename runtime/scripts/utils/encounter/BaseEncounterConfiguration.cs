using Godot;
using Godot.Collections;
[GlobalClass]
public partial class BaseEncounterConfiguration : Resource
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
    public string EncounterBattleFirstState = "";
    [Export]
    public string[] EnemysList = [ "MyEnemy" ];
    [Export]
    public bool CanFree = true;
}
