using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerDialogState : StateNode
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
		BattleManager.Instance.GetPlayerSoul().Visible = true;
	}

	private void NextStep()
	{
		BattlePlayerSoul soul = BattleManager.Instance.GetPlayerSoul();
		soul.Visible = false;
		if (DialogueQueueManager.Instance.DialogueCount() > 0)
		{
			string dialogueText = DialogueQueueManager.Instance.GetNextDialogueAsText();
			TextMenu.ShowEncounterText($"* {dialogueText}");
		}
		else
		{
			if (!BattleManager.Instance.Endded)
				EmitSignal(SignalName.RequestSwitchState, ["BattleEnemyDialogueState"]);
		}
	}
}
