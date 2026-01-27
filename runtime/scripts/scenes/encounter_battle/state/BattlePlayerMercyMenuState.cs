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
	private string _freeText = "";

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
					GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
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
					GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
				}

				MercyChoiceMenu.SetChoice(MercyChoice);
			}
			else if (Input.IsActionJustPressed("cancel"))
			{
				SwitchState("BattlePlayerChoiceActionState");
			}
			else if (Input.IsActionJustPressed("confirm"))
			{
				GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStreamFormLibrary("SELECT"));
				string choiced = (string)MercyChoiceMenu.GetChoicedItemId();
				if (choiced == "SPARE")
				{
					foreach (BaseEnemy enemy in GlobalBattleManager.Instance.EnemysList)
					{
						if (enemy.AllowSpare && enemy.CanSpare)
						{
							enemy.OnSpare();
						}
					}
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
		MercyChoiceMenu.SetChoice(MercyChoice);
	}
	public override void _ExitState()
	{
	}

	private async void _OpenTextMenu()
	{
		DialogueQueueManager.Instance.AppendDialogue(_freeText);
		await MenuManager.OpenMenu("EncounterTextMenu");
	}

	private void _Free()
	{ 
		_freed = true;
		GlobalBattleManager.Instance.Endded = true;
		_playerSoul = GlobalBattleManager.Instance.GetPlayerSoul();
		_playerSoul.Freed = true;
		_playerSoul.Visible = true;
		_OpenTextMenu();
		TextMenu.ShowEncounterText(GlobalBattleManager.Instance.FreeText);
		BattleButtonManager.ResetAllBattleButton();
		GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStreamFormLibrary("ESCAPED"));
	}
}
