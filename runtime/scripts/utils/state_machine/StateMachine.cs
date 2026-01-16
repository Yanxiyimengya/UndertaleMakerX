using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class StateMachine : Node
{
	[Export]
	public string CurrentStateName
	{
		get => currentStateName;
		set
		{
			if (stateNodes.TryGetValue(currentStateName, out StateNode prevStateNode) && prevStateNode != null)
			{
				prevStateNode._ExitState();
				prevStateNode.Enabled = false;
			}
			if (stateNodes.TryGetValue(value, out StateNode nextStateNode) && nextStateNode != null)
			{
				nextStateNode.Enabled = true;
				nextStateNode._EnterState();
				currentStateName = value;
			}

			else if (string.IsNullOrEmpty(value))
			{
				currentStateName = string.Empty;
			}
		}
	}

	private Dictionary<string, StateNode> stateNodes = new Dictionary<string, StateNode>();
	private string currentStateName = string.Empty;

	public override void _Ready()
	{
		base._Ready();

		ChildEnteredTree += OnChildEnteredTree;
		ChildExitingTree += OnChildExitingTree;

		foreach (Node childNode in GetChildren())
		{
			AddStateNodeIfValid(childNode);
		}

		if (!string.IsNullOrEmpty(CurrentStateName))
		{
			CurrentStateName = CurrentStateName;
		}
	}
	private void OnChildEnteredTree(Node child)
	{
		AddStateNodeIfValid(child);
	}
	private void OnChildExitingTree(Node child)
	{
		if (child is StateNode stateNode)
		{
			string stateName = stateNode.Name;
			if (stateName == currentStateName)
			{
				stateNode._ExitState();
				stateNode.Enabled = false;
				currentStateName = string.Empty;
			}
			if (stateNodes.ContainsKey(stateName))
			{
				stateNodes.Remove(stateName);
			}
		}
	}

	private void AddStateNodeIfValid(Node node)
	{
		if (node is StateNode stateNode && !stateNodes.ContainsKey(stateNode.Name))
		{
			stateNodes.Add(stateNode.Name, stateNode);
			stateNode.Enabled = false;
			stateNode.Connect(
				StateNode.SignalName.RequestSwitchState,
				Callable.From((string stateName) => SwitchToState(stateName))
			);
		}
	}
	public bool SwitchToState(string stateName)
	{
		if (stateNodes.ContainsKey(stateName))
		{
			if (stateNodes.TryGetValue(stateName, out StateNode nextStateNode) && nextStateNode != null)
			{
				if (nextStateNode._CanEnterState())
				{
					CurrentStateName = stateName;
					return true;
				}
			}
		}
		return false;
	}
	public bool HasState(string stateName)
	{
		return stateNodes.ContainsKey(stateName);
	}
	public override void _ExitTree()
	{
		base._ExitTree();
		ChildEnteredTree -= OnChildEnteredTree;
		ChildExitingTree -= OnChildExitingTree;
	}
}
