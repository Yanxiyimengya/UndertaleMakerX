using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

[GlobalClass]
[Tool]
public partial class StateMachine : Node
{
	[Export]
	public int CurrentStateIndex
	{
		get => _currentStateIndex;
		set
		{
			TryGetState(_currentStateIndex, out StateNode prevStateNode);
			if (prevStateNode != null)
			{
				prevStateNode._ExitState();
				prevStateNode.Enabled = false;
			}

			TryGetState(value, out StateNode nextState);
			if (nextState != null)
			{
				
				TryGetState(value, out StateNode currentStateNode);
				currentStateNode.Enabled = true;
				nextState._EnterState();
			}
			_currentStateIndex = value;

		}
	}

	private List<StateNode> stateNodes = new List<StateNode>();
	private int _currentStateIndex = -1;
	
	public override void _Ready()
	{
		base._Ready();
		foreach (Node childNode in this.GetChildren())
		{
			if (! (childNode is StateNode)) continue;
			stateNodes.Add((StateNode)childNode);
		}
		CurrentStateIndex = CurrentStateIndex;
	}
	public bool TryGetState(int idx, out StateNode state)
	{
		state = null;
		if (idx >= 0 && idx < stateNodes.Count)
		{
			state = stateNodes[idx];
			return true;
		}
		return false;
	}
}
