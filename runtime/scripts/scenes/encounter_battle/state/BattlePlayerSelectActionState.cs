using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BattlePlayerSelectActionState : StateNode
{
	[Export]
	AudioStream SndSelect;
	[Export]
	AudioStream SndSqueak;
	
	[Export]
	BattlePlayerSoul BattlePlayerSoul;
	[Export]
	BattleScreenButtonManager BattleButtonManager;
	[Export]
	BattleMenuManager BattleMenuManager;

	private int actionSelect = 0;
	private int prevActionSelect = -1;
	private Dictionary<int, string> actionButtonMapping = new Dictionary<int, string>
	{
		{ 0 , "FightButton"},
		{ 1 , "ActButton"},
		{ 2 , "ItemButton"},
		{ 3 , "MercyButton"},
	}; // 按钮映射表
	private Dictionary<int, string> actionMenuMapping = new Dictionary<int, string>
	{
		{ 0 , "BattlePlayerSelectEnemyState"},
		{ 1 , "BattlePlayerSelectEnemyState"},
		{ 2 , "BattlePlayerSelectItemState"},
		{ 3 , "MercyButton"},
	}; // 菜单行为映射表

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left"))
		{
			actionSelect -= 1;
			GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			if (actionSelect < 0) actionSelect = BattleButtonManager.GetButtonCount() - 1;
		}
		else if (Input.IsActionJustPressed("right"))
		{
			actionSelect += 1;
			GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			if (actionSelect >= BattleButtonManager.GetButtonCount()) actionSelect = 0;
		}
		else if (Input.IsActionJustPressed("confirm"))
		{
			if (actionMenuMapping.TryGetValue(actionSelect, out string state))
			{
				EmitSignal(SignalName.RequestSwitchState, [state]);
			}

			GlobalStreamPlayer.Instance.PlaySound(SndSelect);
		}

		if (actionSelect != prevActionSelect) { 
			SetSelect(actionSelect);
			prevActionSelect = actionSelect;
		}
	}

	public void SetSelect(int select)
	{
		if (actionButtonMapping.TryGetValue(actionSelect, out string btnId))
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
		SetSelect(actionSelect);
		BattlePlayerSoul.Movable = false;
		BattlePlayerSoul.EnableCollision = false;
		BattlePlayerSoul.Show();
	}

	public override void _ExitState()
	{
		if (actionButtonMapping.TryGetValue(actionSelect, out string btnId))
		{
			BattleButtonManager.ReleaseButton(btnId);
		}
	}
}
