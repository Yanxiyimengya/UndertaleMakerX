using Godot;
using System;

[GlobalClass]
public partial class EncounterConfiguration : Resource
{
	[Export(PropertyHint.MultilineText)]
	public string EncounterText = "* UndertaleMaker[color=aqua]X[/color]!";
    [Export(PropertyHint.MultilineText)]
    public string FreeText = "";
    [Export(PropertyHint.MultilineText)]
    public string DeathText = "* 还不能放弃...[waitfor=confirm][clear]保持你的决心吧";
    [Export(PropertyHint.MultilineText)]
    public string EndText = "";

    [Export]
	public string EncounterBattleFirstState = "";
    [Export]
    public bool CanFree = true;
}
