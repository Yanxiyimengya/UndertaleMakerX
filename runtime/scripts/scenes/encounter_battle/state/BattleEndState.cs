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
	
	public override async void _EnterState()
	{
		await MenuManager.OpenMenu("EncounterTextMenu");
		BattleButtonManager.ResetAllBattleButton();
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			TextMenu.ShowEncounterText(BattleManager.Instance.EndText);
		}
	}

	public override void _ExitState()
	{
		BattleManager.Instance.GetPlayerSoul().Visible = true;
	}
}
