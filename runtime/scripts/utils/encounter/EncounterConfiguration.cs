using Godot;
using System;

[GlobalClass]
public partial class EncounterConfiguration : Resource
{
	[Export(PropertyHint.MultilineText)]
	public string DefaultEncounterText = "* UndertaleMaker[color=aqua]X[/color]!";
    [Export(PropertyHint.MultilineText)]
    public string FreeText = "";
    [Export(PropertyHint.MultilineText)]
    public string EndText = "";

    [Export]
	public string EncounterBattleFirstState = "";
    [Export]
    public bool CanFree = true;
}
