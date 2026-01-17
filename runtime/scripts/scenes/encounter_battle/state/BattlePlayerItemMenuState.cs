using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerItemMenuState : StateNode
{
	[Export]
	AudioStream SndSelect;
	[Export]
	AudioStream SndSqueak;
	
	[Export]
	BattleMenuManager MenuManager;
	[Export]
	EncounterItemMenu ItemChoiceMenu;

	public int ItemChoice = 0;
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("up"))
		{
			ItemChoice -= 1;
			if (ItemChoice < 0)
			{
				ItemChoice = 0;
			}
			else 
			{
				GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			}
			ItemChoiceMenu.SetChoice(ItemChoice);

		}
		else if (Input.IsActionJustPressed("down"))
		{
			ItemChoice += 1;
			if (ItemChoice >= PlayerDataManager.Instance.GetItemCount())
			{
				ItemChoice = PlayerDataManager.Instance.GetItemCount() - 1;
			}
			else
			{
				GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			}
			ItemChoiceMenu.SetChoice(ItemChoice);
		}
		else if (Input.IsActionJustPressed("cancel"))
		{
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
		}
	}

	public override async void _EnterState()
	{
		await MenuManager.OpenMenu("EncounterItemMenu");
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		ItemChoiceMenu.SetChoice(ItemChoice);


	}

	public override void _ExitState()
	{
	}

	public override bool _CanEnterState()
	{
		return PlayerDataManager.Instance.GetItemCount() > 0;
	}
}
