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

	public override void _Process(double delta)
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
			//ItemChoiceMenu.SetChoice(MercyChoice);

		}
		else if (Input.IsActionJustPressed("down"))
		{
			MercyChoice += 1;
			if (MercyChoice >= PlayerDataManager.Instance.GetItemCount())
			{
				MercyChoice = PlayerDataManager.Instance.GetItemCount() - 1;
			}
			else
			{
				GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			}
			//ItemChoiceMenu.SetChoice(MercyChoice);
		}
		else if (Input.IsActionJustPressed("cancel"))
		{
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
		}
	}
	
	public override async void _EnterState()
	{
		await MenuManager.OpenMenu("EncounterMercyMenu");
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		MercyChoiceMenu.SetChoice(MercyChoice);
	}
	public override void _ExitState()
	{
	}
}
