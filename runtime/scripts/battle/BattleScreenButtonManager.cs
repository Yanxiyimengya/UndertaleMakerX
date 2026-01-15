using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BattleScreenButtonManager : Node
{
	private List<BattleScreenButton> buttonList = new List<BattleScreenButton>();
	private int _prevButtonIndex = -1;

	public override void _Ready()
	{
		base._Ready();
		foreach (Node childNode in this.GetChildren())
		{
			if (!(childNode is BattleScreenButton btn)) continue;
			buttonList.Add(btn);
		}
	}

	public void ReleaseButton(int ind)
	{
		if (ind > -1 && ind < buttonList.Count)
		{
			_prevButtonIndex = ind;
			GetButton(ind).Pressed = false;
		}
	}

	public void PressButton(int ind)
	{
		if (ind == _prevButtonIndex) return;
		if (ind > -1 && ind < buttonList.Count)
		{
			if (_prevButtonIndex > -1 && _prevButtonIndex < buttonList.Count)
			{
				GetButton(_prevButtonIndex).Pressed = false;
			}
			GetButton(ind).Pressed = true;
			_prevButtonIndex = ind;
		}
	}

	public BattleScreenButton GetButton(int ind) { 
		return buttonList[ind];
	}

	public int GetButtonCount()
	{
		return buttonList.Count;
	}
}
