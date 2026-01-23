using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class BattleScreenButtonManager : Node2D
{
	private System.Collections.Generic.Dictionary<string, BattleScreenButton> buttonList = 
				new Dictionary<string, BattleScreenButton>();
	private string _prevButtonId= "";

	public override void _Ready()
	{
		base._Ready();
		foreach (Node childNode in this.GetChildren())
		{
			if (!(childNode is BattleScreenButton btn)) continue;
			buttonList.Add(btn.Name, btn);
		}
	}

	public void ReleaseAllButton()
	{
		foreach (KeyValuePair<string, BattleScreenButton> pair in buttonList)
		{
			pair.Value.Pressed = false;
		}
	}

	public void ReleaseButton(string id)
	{
		if (GetButton(id, out BattleScreenButton btn))
		{
			_prevButtonId = id;
			btn.Pressed = false;
		}
	}

	public void PressButton(string id)
	{
		if (buttonList.TryGetValue(id, out BattleScreenButton btn))
		{
			if (GetButton(_prevButtonId, out BattleScreenButton prevBtn))
				prevBtn.Pressed = false;
			btn.Pressed = true;
			_prevButtonId = id;
		}
	}

	public bool GetButton(string id, out BattleScreenButton button) {

		if (buttonList.TryGetValue(id, out button))
			return true;
		return false;
	}

	public string GetPrevButtonId()
	{
		return _prevButtonId;
	}

	public int GetButtonCount()
	{
		return buttonList.Count;
	}
}
