using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerDialogueState : StateNode
{
	[Export]
	BattleMenuManager MenuManager;
	[Export]
	EncounterTextMenu TextMenu;
	[Export]
	BattleScreenButtonManager BattleButtonManager;
	public override void _Process(double delta)
	{
		if (TextMenu.IsTextTyperFinished())
		{
			if (Input.IsActionJustPressed("confirm"))
			{
				NextStep();
			}
		}
	}

	public override async void _EnterState()
	{
		NextStep();
		await MenuManager.OpenMenu("EncounterTextMenu");
		BattleButtonManager.ResetAllBattleButton();
	}

	public override void _ExitState()
	{
		UtmxBattleManager.GetBattlePlayerController().PlayerSoul.Visible = true;
	}

	private void NextStep()
	{
		BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
		soul.Visible = false;
		if (UtmxDialogueQueueManager.DialogueCount() > 0)
		{
			string dialogueText = UtmxDialogueQueueManager.GetNextDialogueAsText();
			TextMenu.ShowEncounterText(dialogueText);
		}
		else
		{
			if (!UtmxBattleManager.Endded)
			{
				UtmxBattleManager.GetBattleController().ChangeToEnemyDialogueState();
			}
		}
	}
}
