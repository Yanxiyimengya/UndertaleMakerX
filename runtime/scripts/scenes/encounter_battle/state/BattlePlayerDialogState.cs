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
		if (TextMenu.TextTyperFinished())
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
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			enc.GetPlayerSoul().Visible = false;
		}
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
		if (DialogueQueueManager.Instance.DialogueCount() > 0)
		{
			string dialogueText = DialogueQueueManager.Instance.GetNextDialogueAsText();
			TextMenu.ShowEncounterText($"* {dialogueText}");
		}
		else 
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
	}
}
