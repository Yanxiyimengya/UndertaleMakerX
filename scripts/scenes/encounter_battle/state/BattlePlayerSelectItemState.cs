using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerChoiceItemState : StateNode
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
		BattleMenuManager.OpenMenu("EncounterItemChoiceMenu");
	}

	public override void _ExitState()
	{
	}

    public override bool _CanEnterState()
    {
        return PlayerDataManager.Instance.GetItemCount() > 0;
    }
}
