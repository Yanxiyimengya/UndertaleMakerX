using Godot;
using System;
using static Godot.WebSocketPeer;

[GlobalClass]
public partial class BattlePlayerSelectEnemyState : StateNode
{
	[Export]
	BattleMenuManager BattleMenuManager;
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("cancel"))
		{
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerSelectActionState"]);
		}
	}

	public override void _EnterState()
	{
		BattleMenuManager.OpenMenu("EncounterEnemySelectMenu");
	}

	public override void _ExitState()
	{
	}
}
