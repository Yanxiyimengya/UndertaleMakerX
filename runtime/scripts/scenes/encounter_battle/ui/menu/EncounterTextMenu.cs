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
	
	private AudioStream _defaultVoice;
	
	public override void _Ready()
	{
		_defaultVoice = GlobalStreamPlayer.Instance.GetStream("TEXT_TYPER_VOICE");
	}
	
	public override void _Process(double delta)
	{
	}
	
	public override void UIVisible()
	{
	}
	public override void UIHidden()
	{
		encounterTextTyper.Start("");
	}

	
	public void ShowEncounterText(string text) 
	{
		encounterTextTyper.TyperFont = DefaultFont;
		encounterTextTyper.TyperSize = 24;
		encounterTextTyper.Voice = _defaultVoice;
		encounterTextTyper.Instant = false;
		if (GetTree().CurrentScene is EncounterBattle enc) 
			encounterTextTyper.Start(text);
	}

	public bool IsTextTyperFinished()
	{
		return encounterTextTyper.IsFinished();
	}

}
