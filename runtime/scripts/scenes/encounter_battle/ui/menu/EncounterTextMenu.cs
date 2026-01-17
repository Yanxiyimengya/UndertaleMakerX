using Godot;
using System;

public partial class EncounterTextMenu : BaseEncounterMenu
{
	[Export]
	TextTyper encounterTextTyper;
	
	[Export]
	Font DefaultFont;
	[Export]
	int FontSize;
	[Export]
	AudioStream DefaultVoice;

	public override void UIVisible()
	{
		if (GetTree().CurrentScene is EncounterBattle enc) 
			ShowEncounterText(enc.EncounterText);
	}
	public override void UIHidden()
	{
		encounterTextTyper.Start("");
	}

	public override void _Process(double delta)
	{

		if (Input.IsActionJustPressed("cancel"))
		{
			encounterTextTyper.Instant = true;
		}
	}
	
	public void ShowEncounterText(string text) 
	{
		encounterTextTyper.TyperFont = DefaultFont;
		encounterTextTyper.TyperSize = FontSize;
		encounterTextTyper.Voice = DefaultVoice;
		encounterTextTyper.Instant = false;
		if (GetTree().CurrentScene is EncounterBattle enc) 
			encounterTextTyper.Start(text);
	}
}
