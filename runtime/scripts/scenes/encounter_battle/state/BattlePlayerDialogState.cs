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
		BattleButtonManager.ReleaseAllButton();
	}

	public override void _ExitState()
	{
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			enc.GetPlayerSoul().Visible = true;
		}
	}

	private void NextStep()
	{
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			BattlePlayerSoul soul = enc.GetPlayerSoul();
			soul.Visible = false;
			if (DialogueQueueManager.Instance.DialogueCount() > 0)
			{
				string dialogueText = DialogueQueueManager.Instance.GetNextDialogueAsText();
				TextMenu.ShowEncounterText($"* {dialogueText}");
			}
			else
			{
				if (! enc.Endded)
					EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
			}
		}
	}
}
