using Godot;
using System;
using System.Threading.Tasks;
using static Godot.WebSocketPeer;

[GlobalClass]
public partial class BattlePlayerChoiceEnemyState : StateNode
{
	[Export]
	BattleMenuManager BattleMenuManager;
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("cancel"))
		{
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
		}
	}

	public override void _EnterState()
	{
		BattleMenuManager.OpenMenu("EncounterEnemyChoiceMenu");
	}

	public override void _ExitState()
	{
	}
}
