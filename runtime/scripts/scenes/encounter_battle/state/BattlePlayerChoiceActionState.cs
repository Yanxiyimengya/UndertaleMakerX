using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BattlePlayerChoiceActionState : StateNode
{
	[Export]
	BattleScreenButtonManager BattleButtonManager;
	[Export]
	EncounterTextMenu TextMenu;
	[Export]
	BattleMenuManager MenuManager;

	public int ActionChoice = 0;
	private Dictionary<int, string> actionButtonMapping = new Dictionary<int, string>
	{
		{ 0 , "FightButton"},
		{ 1 , "ActButton"},
		{ 2 , "ItemButton"},
		{ 3 , "MercyButton"},
	}; // 按钮映射表
	private Dictionary<int, string> actionMenuMapping = new Dictionary<int, string>
	{
		{ 0 , "BattlePlayerFightMenuState"},
		{ 1 , "BattlePlayerActMenuState"},
		{ 2 , "BattlePlayerItemMenuState"},
		{ 3 , "BattlePlayerMercyMenuState"},
	}; // 菜单行为映射表

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left"))
		{
			ActionChoice -= 1;
			GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStream("SQUEAK"));
			if (ActionChoice < 0) ActionChoice = BattleButtonManager.GetButtonCount() - 1;
		}
		else if (Input.IsActionJustPressed("right"))
		{
			ActionChoice += 1;
			GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStream("SQUEAK"));
			if (ActionChoice >= BattleButtonManager.GetButtonCount()) ActionChoice = 0;
		}
		else if (Input.IsActionJustPressed("confirm"))
		{
			if (TryGetActionMenuMapping(ActionChoice, out string state))
			{
				EmitSignal(SignalName.RequestSwitchState, [state]);
			}
			GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStream("SELECT"));
		}
		SetChoice(ActionChoice);
	}

	public void SetChoice(int Choice)
	{
		if (TryGetActionButtonMapping(ActionChoice, out string btnId))
		{
			BattlePlayerSoul soul = BattleManager.Instance.GetPlayerSoul();
			if (BattleButtonManager.GetButton(btnId, out BattleScreenButton btn))
			{
				soul.GlobalTransform = btn.GetSoulTransform();
			}
			BattleButtonManager.PressButton(btnId);
		}
	}

	public override async void _EnterState()
    {
        await MenuManager.OpenMenu("EncounterTextMenu");
		SetChoice(ActionChoice);
		
		TextMenu.ShowEncounterText(BattleManager.Instance.EncounterText);
		BattlePlayerSoul soul = BattleManager.Instance.GetPlayerSoul();
		soul.Movable = false;
		soul.Show();
	}

	public override void _ExitState()
	{
	}

	public bool TryGetActionButtonMapping(int ind,out string state)
	{
		return actionButtonMapping.TryGetValue(ind, out state);
	}
	public bool TryGetActionMenuMapping(int ind, out string state)
	{
		return actionMenuMapping.TryGetValue(ind, out state);
	}
}
