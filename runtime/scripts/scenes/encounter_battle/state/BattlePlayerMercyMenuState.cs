using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerMercyMenuState : StateNode
{
	[Export]
	BattleMenuManager MenuManager;
	[Export]
	EncounterTextMenu TextMenu;
	[Export]
	BattleScreenButtonManager BattleButtonManager;
	[Export]
	EncounterMercyMenu MercyChoiceMenu;

	public int MercyChoice = 0;

	private bool _freed = false;
	private BattlePlayerSoul _playerSoul;

	public override void _Process(double delta)
	{
		if (_freed)
		{
			if (_playerSoul != null)
			{
				_playerSoul.Position = new Vector2(_playerSoul.Position.X - (float)(delta * 120),
					_playerSoul.Position.Y);
			}
			if (TextMenu.IsTextTyperFinished())
			{
				if (Input.IsActionJustPressed("confirm"))
				{
					UtmxBattleManager.EncounterBattleEnd();
				}
			}
		}
		else
		{

			if (Input.IsActionJustPressed("up"))
			{
				MercyChoice -= 1;
				if (MercyChoice < 0)
				{
					MercyChoice = 0;
				}
				else
				{
					UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
				}
				MercyChoiceMenu.SetChoice(MercyChoice);
			}
			else if (Input.IsActionJustPressed("down"))
			{
				MercyChoice += 1;
				if (MercyChoice >= MercyChoiceMenu.GetItemCount())
				{
					MercyChoice = MercyChoiceMenu.GetItemCount() - 1;
				}
				else
				{
					UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
				}

				MercyChoiceMenu.SetChoice(MercyChoice);
			}
			else if (Input.IsActionJustPressed("cancel"))
			{
				SwitchState("BattlePlayerChoiceActionState");
			}
			else if (Input.IsActionJustPressed("confirm"))
			{
				UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SELECT"));
				string choiced = (string)MercyChoiceMenu.GetChoicedItemId();
				if (choiced == "SPARE")
				{
					foreach (BaseEnemy enemy in UtmxBattleManager.Instance.GetBattleEnemyController().EnemyList)
					{
						if (enemy.AllowSpare && enemy.CanSpare)
						{
							enemy._OnSpare();
						}
					}
					_NextState();
				}
				else if (choiced == "FREE")
				{
					_Free();
				}
			}
		}
	}

	public override async void _EnterState()
	{
		_freed = false;
		await MenuManager.OpenMenu("EncounterMercyMenu");
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		MercyChoice = Math.Clamp(MercyChoice, 0, MercyChoiceMenu.GetItemCount() - 1);
		MercyChoiceMenu.SetChoice(MercyChoice);
	}
	public override void _ExitState()
	{
	}
	
	private void _NextState()
	{
		UtmxBattleManager.Instance.GetBattleController().ChangeToPlayerDialogueState();
	}

	private async void _OpenTextMenu()
	{
		await MenuManager.OpenMenu("EncounterTextMenu");
	}

	private void _Free()
	{
		_freed = true;
		UtmxBattleManager.Instance.Endded = true;
		_playerSoul = UtmxBattleManager.Instance.GetBattleController().PlayerSoul;
		_playerSoul.Freed = true;
		_playerSoul.Visible = true;
		_OpenTextMenu();
		TextMenu.ShowEncounterText(UtmxBattleManager.Instance.GetEncounterInstance()?.FreeText);
		BattleButtonManager.ResetAllBattleButton();
		UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("ESCAPED"));
	}
}
