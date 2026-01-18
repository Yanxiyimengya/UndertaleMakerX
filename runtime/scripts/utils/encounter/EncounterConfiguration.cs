using Godot;
using System;

[GlobalClass]
public partial class EncounterConfiguration : Resource
{
	[Export(PropertyHint.MultilineText)]
	public string DefaultEncounterText = "";
	[Export]
	public string EncounterBattleFirstState = "";
    [Export]
    public bool CanFree = true;
}
