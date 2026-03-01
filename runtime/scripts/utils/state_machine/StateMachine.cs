using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class StateMachine : Node
{
	[Export]
	public string CurrentStateName
	{
		get => _currentStateName;
		set
		{
			if (_stateNodes.TryGetValue(_currentStateName, out StateNode prevStateNode) && prevStateNode != null)
			{
				prevStateNode._ExitState();
				prevStateNode.Enabled = false;
			}
			if (_stateNodes.TryGetValue(value, out StateNode nextStateNode) && nextStateNode != null)
			{
				nextStateNode.Enabled = true;
				nextStateNode._EnterState();
				_currentStateName = value;
			}
			else if (string.IsNullOrEmpty(value))
			{
				_currentStateName = string.Empty;
			}
		}
	}

	private Dictionary<string, StateNode> _stateNodes = new Dictionary<string, StateNode>();
	private string _currentStateName = string.Empty;

	public override void _Ready()
	{
		foreach (Node childNode in GetChildren())
		{
			AddStateNodeIfValid(childNode);
		}

		if (!string.IsNullOrEmpty(CurrentStateName))
		{
			CurrentStateName = CurrentStateName;
		}
		else
		{
			CurrentStateName = GetChild(0).Name;
		}
	}

	private void AddStateNodeIfValid(Node node)
	{
		if (node is StateNode stateNode && !_stateNodes.ContainsKey(stateNode.Name))
		{
			_stateNodes.Add(stateNode.Name, stateNode);
			stateNode.Enabled = false;
			stateNode.Connect(
				StateNode.SignalName.RequestSwitchState,
				Callable.From((string stateName) => SwitchToState(stateName))
			);
		}
	}
	public void SwitchToState(string stateName)
	{
		if (_stateNodes.TryGetValue(stateName, out StateNode nextStateNode) && nextStateNode != null)
		{
			if (nextStateNode._CanEnterState())
			{
				CurrentStateName = stateName;
			}
		}
	}
	public bool HasState(string stateName)
	{
		return _stateNodes.ContainsKey(stateName);
	}
}
