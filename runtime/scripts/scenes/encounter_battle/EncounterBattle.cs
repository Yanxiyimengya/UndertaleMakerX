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
	Node2D EnemysNode;
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


	public override void _EnterTree()
	{
		foreach (BaseEnemy enemy in Enemys)
		{
			EnemysNode.AddChild(enemy);
		}	
	}

	public override void _Ready()
	{
		BattleStateMachine.SwitchToState(encounterConfig.EncounterBattleFirstState);
	}
	public BattlePlayerSoul GetPlayerSoul()
	{
		return PlayerSoul;
	}
}
