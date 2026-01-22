using Godot;
using System;

[GlobalClass]
public partial class BattleEnemyDialogueState : StateNode
{
	[Export]
	public PackedScene DialogueSpeechBubble;
	[Export]
	public BattleMenuManager MenuManager;

	public override void _Process(double delta)
	{
	}

	public override void _EnterState()
	{
		MenuManager.CloseAllMenu();
	}

	public override void _ExitState()
	{
		NextStep();
	}


	private void NextStep()
	{
		if (DialogueQueueManager.Instance.DialogueCount() > 0)
		{
			string dialogueText = DialogueQueueManager.Instance.GetNextDialogueAsText();
		}
		else
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
	}
}
