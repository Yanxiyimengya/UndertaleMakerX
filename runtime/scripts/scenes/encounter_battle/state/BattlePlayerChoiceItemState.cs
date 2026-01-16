using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerChoiceItemState : StateNode
{
	[Export]
	AudioStream SndChoice;
	[Export]
	AudioStream SndSqueak;
	
	[Export]
	BattleMenuManager BattleMenuManager;
	[Export]
	EncounterItemChoiceMenu encounterItemChoiceMenu;

	public int itemChoice = 0;
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("up"))
		{
			itemChoice -= 1;
			if (itemChoice < 0)
			{
				itemChoice = 0;
			}
			else 
			{
				GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			}
			encounterItemChoiceMenu.SetChoice(itemChoice);

		}
		else if (Input.IsActionJustPressed("down"))
		{
			itemChoice += 1;
			if (itemChoice >= PlayerDataManager.Instance.GetItemCount())
			{
				itemChoice = PlayerDataManager.Instance.GetItemCount() - 1;
			}
			else
			{
				GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			}
			encounterItemChoiceMenu.SetChoice(itemChoice);
		}
		else if (Input.IsActionJustPressed("cancel"))
		{
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
		}
	}

	public override void _EnterState()
	{
		BattleMenuManager.OpenMenu("EncounterItemChoiceMenu");
	}

	public override void _ExitState()
	{
	}

	public override bool _CanEnterState()
	{
		return PlayerDataManager.Instance.GetItemCount() > 0;
	}
}
