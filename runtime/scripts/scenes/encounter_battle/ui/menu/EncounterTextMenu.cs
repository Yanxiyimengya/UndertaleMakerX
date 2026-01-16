using Godot;
using System;

public partial class EncounterTextMenu : BaseEncounterMenu
{
	[Export]
	TextTyper encounterTextTyper;
	
	[Export]
	Font DefaultFont;
	[Export]
	AudioStream DefaultVoice;

	public override void UIVisible()
	{
		encounterTextTyper.TyperFont = DefaultFont;
		encounterTextTyper.Voice = DefaultVoice;
		if (GetTree().CurrentScene is EncounterBattle enc) 
			encounterTextTyper.Start(enc.EncounterText);
	}
	public override void UIHidden()
	{
		encounterTextTyper.Start("");
	}
}
