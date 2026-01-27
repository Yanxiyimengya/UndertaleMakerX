using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerItemMenuState : StateNode
{
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
				GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
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
				GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
			}
			ItemChoiceMenu.SetChoice(ItemChoice);
		}
		else if (Input.IsActionJustPressed("cancel"))
		{
			SwitchState("BattlePlayerChoiceActionState");
		}
		else if (Input.IsActionJustPressed("confirm"))
		{
			int itemChoiced = (int)ItemChoiceMenu.GetChoicedItemId();
			PlayerDataManager.Instance.UseItem(itemChoiced);
			SwitchState("BattlePlayerDialogState");
		}
	}

	public override async void _EnterState()
	{
		await MenuManager.OpenMenu("EncounterItemMenu");
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
