using Godot;
using System;

[GlobalClass]
public abstract partial class StateNode : Node
{
	[Signal]
	public delegate void RequestSwitchStateEventHandler(string stateName);

	[Export]
	public bool Enabled
	{
		get => _enabled;
		set
		{
			_enabled = value;
			if (value)
			{
				ProcessMode = ProcessModeEnum.Inherit;
				SetPhysicsProcess(true);
			}
			else
			{
				ProcessMode = ProcessModeEnum.Disabled;
				SetPhysicsProcess(false);
			}
		}
	}
	private bool _enabled;

	public override void _Notification(int what)
	{
		base._Notification(what);
		if (what == NotificationReady)
		{
			Enabled = false;
		}
	}

	// 进入自身状态的回调方法
	public abstract void _EnterState();

	// 退出自身状态的回调方法
	public abstract void _ExitState();

    public virtual bool _CanEnterState()
    {
        return true;
    }
}
