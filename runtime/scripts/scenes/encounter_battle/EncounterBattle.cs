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
	BattleRectangleArenaExpand MainArena;
	[Export]
	StateMachine BattleStateMachine;
	[Export]
	BattleStatusBar StatusBar;
	[Export]
	Node2D EnemysNode;
	[Export]
	EncounterConfiguration Config
	{
		get => _encounterConfig;
		set
		{
			_encounterConfig = value;
			CanFree = _encounterConfig.CanFree;
			EncounterText = _encounterConfig.DefaultEncounterText;
			FreeText = _encounterConfig.FreeText;
		}
	}

	public string EncounterText = "";
	public string FreeText = "";
	public List<BaseEnemy> Enemys = [];
	public bool CanFree;
	public bool Endded = false;

	private EncounterConfiguration _encounterConfig = null;


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
		BattleStateMachine.SwitchToState(_encounterConfig.EncounterBattleFirstState);
		AddEnemy(new BaseEnemy(), new Vector2(0F, -0F));
		AddEnemy(new BaseEnemy(), new Vector2(0F, -0F));
		AddEnemy(new BaseEnemy(), new Vector2(0F, -0F));
		AddEnemy(new BaseEnemy(), new Vector2(0F, -0F));
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
