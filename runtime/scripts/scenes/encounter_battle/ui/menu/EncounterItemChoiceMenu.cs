using Godot;
using System;

public partial class EncounterItemChoiceMenu : BaseEncounterMenu
{
	[Export]
	UndertaleStyleScrollBar scrollBar;
	[Export]
	Godot.Collections.Array<TextTyper> itemTextTypers = [null, null, null];
	[Export]
	Godot.Collections.Array<Marker2D> soulMarker = [null, null, null];

	private int firstIndex = 0;
	
	public override void UIVisible()
	{
		scrollBar.Count = PlayerDataManager.Instance.GetItemCount();
		SetChoice(firstIndex);
	}
	public override void UIHidden()
	{

	}
	
	public void SetChoice(int Choice)
	{
		firstIndex = Math.Max(0, Choice - Math.Min(2, Choice));
		scrollBar.CurrentIndex = Choice;
		for (var i = 0; i < 3; i++)
		{
			int slot = (i + firstIndex);
			TextTyper typer = itemTextTypers[i];
			string displayText = "";
			if (slot < PlayerDataManager.Instance.GetItemCount())
			{
				displayText = $"* {PlayerDataManager.Instance.Items[slot].DisplayName}{slot}";

				if (Choice == slot) {
					if (GetTree().CurrentScene is EncounterBattle enc)
					{
						BattlePlayerSoul soul = enc.GetPlayerSoul();
						soul.GlobalPosition = soulMarker[slot].GlobalPosition;
					}
				}
			}
			typer.Start(displayText);
		}
	}
}
