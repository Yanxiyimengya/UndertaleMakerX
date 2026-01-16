using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BattlePlayerChoiceActionState : StateNode
{
	[Export]
	AudioStream SndChoice;
	[Export]
	AudioStream SndSqueak;
	
	[Export]
	BattlePlayerSoul BattlePlayerSoul;
	[Export]
	BattleScreenButtonManager BattleButtonManager;
	[Export]
	BattleMenuManager BattleMenuManager;

	private int actionChoice = 0;
	private int prevActionChoice = -1;
	private Dictionary<int, string> actionButtonMapping = new Dictionary<int, string>
	{
		{ 0 , "FightButton"},
		{ 1 , "ActButton"},
		{ 2 , "ItemButton"},
		{ 3 , "MercyButton"},
	}; // 按钮映射表
	private Dictionary<int, string> actionMenuMapping = new Dictionary<int, string>
	{
		{ 0 , "BattlePlayerChoiceEnemyState"},
		{ 1 , "BattlePlayerChoiceEnemyState"},
		{ 2 , "BattlePlayerChoiceItemState"},
		{ 3 , "MercyButton"},
	}; // 菜单行为映射表

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left"))
		{
			actionChoice -= 1;
			GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			if (actionChoice < 0) actionChoice = BattleButtonManager.GetButtonCount() - 1;
		}
		else if (Input.IsActionJustPressed("right"))
		{
			actionChoice += 1;
			GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			if (actionChoice >= BattleButtonManager.GetButtonCount()) actionChoice = 0;
		}
		else if (Input.IsActionJustPressed("confirm"))
		{
			if (actionMenuMapping.TryGetValue(actionChoice, out string state))
			{
				EmitSignal(SignalName.RequestSwitchState, [state]);
			}

			GlobalStreamPlayer.Instance.PlaySound(SndChoice);
		}

		if (actionChoice != prevActionChoice) { 
			SetChoice(actionChoice);
			prevActionChoice = actionChoice;
		}
	}

	public void SetChoice(int Choice)
	{
		if (actionButtonMapping.TryGetValue(actionChoice, out string btnId))
		{
			if (BattlePlayerSoul != null)
			{
				if (BattleButtonManager.GetButton(btnId, out BattleScreenButton btn))
				{
					BattlePlayerSoul.GlobalPosition = btn.GetSoulPosition();
				}
			}
			BattleButtonManager.PressButton(btnId);
		}
	}

	public override void _EnterState()
	{
		BattleMenuManager.OpenMenu("EncounterTextMenu");
		SetChoice(actionChoice);
		BattlePlayerSoul.Movable = false;
		BattlePlayerSoul.EnableCollision = false;
		BattlePlayerSoul.Show();
	}

	public override void _ExitState()
	{
		if (actionButtonMapping.TryGetValue(actionChoice, out string btnId))
		{
			BattleButtonManager.ReleaseButton(btnId);
		}
	}
}
