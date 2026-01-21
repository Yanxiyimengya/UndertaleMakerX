using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EncounterBattle : Node
{
	
	[Export]
	BaseWeapon We;
	
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
	public List<BaseEnemy> Enemys = [];
	public bool CanFree;
	
	private EncounterConfiguration encounterConfig = null;


	public override void _EnterTree()
	{
		foreach (BaseEnemy enemy in Enemys)
		{
			EnemysNode.AddChild(enemy);
		}
		PlayerDataManager.Instance.Weapon = We;

	}

	public override void _Ready()
	{
		BattleStateMachine.SwitchToState(encounterConfig.EncounterBattleFirstState);
		AddEnemy(new BaseEnemy(), new Vector2(0F , -0F));
	}
	public BattlePlayerSoul GetPlayerSoul()
	{
		return PlayerSoul;
	}

	public void AddEnemy(BaseEnemy enemy, Vector2 position)
	{
		Enemys.Add(enemy);
		if (! enemy.IsInsideTree()) {
			EnemysNode.AddChild(enemy);
			enemy.Position = position;
		}
	}
}
