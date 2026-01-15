using Godot;
using System;

public partial class EncounterBattle : Node
{
	[Export]
	BattlePlayerSoul soul;
	[Export]
	BattleArena mainArena;
	[Export]
	StateMachine battleStateMachine;

	enum BattleTurn
	{
		Player = 0,
		Dialog = 1,
		Enemy = 2,
		Unknown = 10,
	};
	
	public override void _Ready()
	{
		battleStateMachine.CurrentStateIndex = 0;
	}

	public override void _Process(double delta)
	{
	}
}
