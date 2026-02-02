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
        encounterTextTyper.Voice = UtmxGlobalStreamPlayer.GetStreamFormLibrary("TEXT_TYPER_VOICE");
        encounterTextTyper.Instant = false;
        encounterTextTyper.Start(text);
    }

    public bool IsTextTyperFinished()
    {
        return encounterTextTyper.IsFinished();
    }

}
