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
				UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
			}
			ItemChoiceMenu.SetChoice(ItemChoice);

		}
		else if (Input.IsActionJustPressed("down"))
		{
			ItemChoice += 1;
			if (ItemChoice >= UtmxPlayerDataManager.GetItemCount())
			{
				ItemChoice = UtmxPlayerDataManager.GetItemCount() - 1;
			}
			else
			{
				UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
			}
			ItemChoiceMenu.SetChoice(ItemChoice);
		}
		else if (Input.IsActionJustPressed("cancel"))
		{
			SwitchState("BattlePlayerChoiceActionState");
		}
		else if (Input.IsActionJustPressed("confirm"))
		{
			int itemselected = (int)ItemChoiceMenu.GetselectedItemId();
			UtmxPlayerDataManager.UseItem(itemselected);
			UtmxBattleManager.GetBattleEnemyController()?.TriggerEnemiesUsedItemCallback();
			_NextState();
		}
	}

	public override async void _EnterState()
	{
		await MenuManager.OpenMenu("EncounterItemMenu");
		ItemChoice = Math.Clamp(ItemChoice, 0, ItemChoiceMenu.GetItemCount() - 1);
		ItemChoiceMenu.SetChoice(ItemChoice);
	}
	private void _NextState()
	{
		UtmxBattleManager.GetBattleController().ChangeToPlayerDialogueState();
	}

	public override void _ExitState()
	{
	}

	public override bool _CanEnterState()
	{
		return UtmxPlayerDataManager.GetItemCount() > 0;
	}
}
