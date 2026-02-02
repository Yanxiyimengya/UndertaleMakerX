using Godot;
using System;

[GlobalClass]
public partial class BattleEndState : StateNode
{
	[Export]
	BattleScreenButtonManager BattleButtonManager;
	[Export]
	EncounterTextMenu TextMenu;
	[Export]
	BattleMenuManager MenuManager;

	public override void _Process(double delta)
	{
		if (TextMenu.IsTextTyperFinished())
		{
			if (Input.IsActionJustPressed("confirm"))
			{
				UtmxBattleManager.EncounterBattleEnd();
			}
		}
	}
	public override async void _EnterState()
	{
		await MenuManager.OpenMenu("EncounterTextMenu");
		BattleButtonManager.ResetAllBattleButton();
		TextMenu.ShowEncounterText(UtmxBattleManager.GetEncounterInstance()?.EndText);
	}

	public override void _ExitState()
	{
		UtmxBattleManager.GetBattlePlayerController().PlayerSoul.Visible = true;
	}
}
