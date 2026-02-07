using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class BattlePlayerActMenuState : StateNode
{
	[Export]
	public BattleMenuManager MenuManager;
	[Export]
	public EncounterChoiceEnemyMenu ChoiceEnemyMenu;
	[Export]
	public EncounterActPageMenu encounterActPageMenu;

	public int EnemyChoice = 0;
	public int ActChoice = 0;
	private bool _selected = false;
	private int _prevActChoice = 0;

	public override async void _Process(double delta)
	{
		if (_selected)
		{
			int menuItemCount = encounterActPageMenu.MenuItems.Count;
			int pageCount = encounterActPageMenu.GetItemCount() / menuItemCount;
			int page = ActChoice / menuItemCount;
			int index = ActChoice % menuItemCount;
			int lineCount = (menuItemCount / 2);
			if (Input.IsActionJustPressed("up"))
			{
				if (index < 2)
				{
					ActChoice += (lineCount - 1) * 2;
				}
				else
				{
					ActChoice -= 2;
				}
			}
			else if (Input.IsActionJustPressed("down"))
			{
				if ((int)(index / 2) == (lineCount - 1))
				{
					ActChoice -= (lineCount - 1) * 2;
				}
				else
				{
					ActChoice += 2;
				}
			}
			if (Input.IsActionJustPressed("right"))
			{
				if (index % 2 == 0)
				{
					ActChoice += 1;
				}
				else
				{
					if (page != (pageCount - 1)) ActChoice += menuItemCount - 1;
					else ActChoice = index - 1;
				}
			}
			else if (Input.IsActionJustPressed("left"))
			{
				if (index % 2 == 1)
				{
					ActChoice -= 1;
				}
				else
				{
					if (page != 0) ActChoice -= menuItemCount - 1;
					else ActChoice = (pageCount - 1) * menuItemCount + index + 1;
				}
			}

			if (ActChoice != _prevActChoice)
			{
				ActChoice = Math.Clamp(ActChoice, 0, encounterActPageMenu.GetItemCount() - 1);
				if (ActChoice != _prevActChoice)
				{
					encounterActPageMenu.SetChoice(ActChoice);
					_prevActChoice = ActChoice;
					UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
				}
			}

			if (Input.IsActionJustPressed("cancel"))
			{
				await _OpenEnemyChoiceMenu();
			}
			else if (Input.IsActionJustPressed("confirm"))
			{
				_NextState();
			}

		}
		else
		{
			if (Input.IsActionJustPressed("up"))
			{
				EnemyChoice -= 1;
				if (EnemyChoice < 0)
				{
					EnemyChoice = 0;
				}
				else
				{
					UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
				}
				ActChoice = Math.Clamp(ActChoice, 0, ChoiceEnemyMenu.GetItemCount() - 1);
				ChoiceEnemyMenu.SetChoice(EnemyChoice);
			}
			else if (Input.IsActionJustPressed("down"))
			{
				EnemyChoice += 1;
				int enemysCount = UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount();
				if (EnemyChoice >= enemysCount)
				{
					EnemyChoice = enemysCount - 1;
				}
				else
				{
					UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
				}
				EnemyChoice = Math.Clamp(EnemyChoice, 0, ChoiceEnemyMenu.GetItemCount() - 1);
				ChoiceEnemyMenu.SetChoice(EnemyChoice);
			}
			else if (Input.IsActionJustPressed("cancel"))
			{
				SwitchState("BattlePlayerChoiceActionState");
			}
			else if (Input.IsActionJustPressed("confirm"))
			{
				await _OpenActMenu();
				UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SELECT"));
			}
		}
	}

	private async Task _OpenActMenu()
	{
		int actionsCount = UtmxBattleManager.GetBattleEnemyController().EnemiesList[EnemyChoice].Actions.Length;
		if (actionsCount > 0)
		{
			_selected = true;
			await MenuManager.OpenMenu("EncounterActPageMenu");
			ActChoice = Math.Clamp(ActChoice, 0, actionsCount);
			_prevActChoice = ActChoice;
			encounterActPageMenu.SetChoice(ActChoice);
		}
	}

	private async Task _OpenEnemyChoiceMenu()
	{
		_selected = false;
		ChoiceEnemyMenu.HpBarSetVisible(false);
		await MenuManager.OpenMenu("EncounterChoiceEnemyMenu");
		EnemyChoice = Math.Clamp(EnemyChoice, 0, UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount());
		ChoiceEnemyMenu.SetChoice(EnemyChoice);
	}

	private void _NextState()
	{
		string actionCommand = encounterActPageMenu.GetselectedDisplayName();
		UtmxBattleManager.GetBattleEnemyController().EnemiesList[EnemyChoice]._HandleAction(actionCommand);
		UtmxBattleManager.GetBattleController().ChangeToPlayerDialogueState();
	}

	public override async void _EnterState()
	{
		await _OpenEnemyChoiceMenu();
	}

	public override void _ExitState()
	{
	}
	public override bool _CanEnterState()
	{
		int enemysCount = UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount();
		return enemysCount > 0;
	}
}
