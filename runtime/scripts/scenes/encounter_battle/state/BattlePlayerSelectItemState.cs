using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerSelectItemState : StateNode
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
		BattleMenuManager.OpenMenu("EncounterItemSelectMenu");
	}

	public override void _ExitState()
	{
	}

    public override bool _CanEnterState()
    {
        return PlayerDataManager.Instance.GetItemCount() > 0;
    }
}
