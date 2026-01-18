using Godot;
using System;
using System.Collections.Generic;

public partial class EncounterBattle : Node
{
	[Export]
	BattlePlayerSoul PlayerSoul;
	[Export]
	BattleArena MainArena;
	[Export]
	StateMachine BattleStateMachine;
	[Export]
	BattleStatusBar StatusBar;
	[Export]
	EncounterConfiguration Config
	{
		get => encounterConfig;
		set
		{
			encounterConfig = value;
			CanFree = encounterConfig.CanFree;
			EncounterText = encounterConfig.DefaultEncounterText;
		}
	}

	public string EncounterText = "";
	public List<BaseEnemy> Enemys = [
		new BaseEnemy(),
		new BaseEnemy(),
		new BaseEnemy(),
		new BaseEnemy(),
		new BaseEnemy(),
		new BaseEnemy(),
		new BaseEnemy(),

		];
	public bool CanFree;
	
	private EncounterConfiguration encounterConfig = null;
	
	enum BattleTurn
	{
		Player = 0,
		Dialog = 1,
		Enemy = 2,
		Unknown = 10,
	};
	
	public override void _Ready()
	{
		BattleStateMachine.SwitchToState(encounterConfig.EncounterBattleFirstState);;
	}

	public override void _Process(double delta)
	{
	}

	public BattlePlayerSoul GetPlayerSoul()
	{
		return PlayerSoul;
	}
}
