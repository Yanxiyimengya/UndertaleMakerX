using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerMercyMenuState : StateNode
{
	[Export]
	AudioStream SndSelect;
	[Export]
	AudioStream SndSqueak;
	[Export]
	BattleMenuManager MenuManager;
	[Export]
	EncounterMercyMenu MercyChoiceMenu;

	public int MercyChoice = 0;

	private bool _freed = false;

	public override void _Process(double delta)
	{
		if (_freed)
		{

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
					GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
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
					GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
				}
				MercyChoiceMenu.SetChoice(MercyChoice);
			}
			else if (Input.IsActionJustPressed("cancel"))
			{
				EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
			}
			else if (Input.IsActionJustPressed("confirm"))
			{
				string choiced = (string)MercyChoiceMenu.GetChoicedItemId();

				if (choiced == "SPARE")
				{
					if (GetTree().CurrentScene is EncounterBattle enc)
					{
						foreach (BaseEnemy enemy in enc.Enemys)
						{
							if (enemy.AllowSpare && enemy.CanSpare)
							{
								enemy.OnSpare();
							}
						}
					}
				}
				EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerDialogState"]);
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
}
