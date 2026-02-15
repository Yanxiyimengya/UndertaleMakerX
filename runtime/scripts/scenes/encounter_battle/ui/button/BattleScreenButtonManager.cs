using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class BattleScreenButtonManager : Node2D
{
	private Dictionary<string, BattleScreenButton> buttonList = new();
	private string _prevButtonId = "";
	private string _currentButtonId = "";

	public override void _Ready()
	{
		buttonList.Clear();
		foreach (Node childNode in this.GetChildren())
		{
			if (!(childNode is BattleScreenButton btn)) continue;
			AddButton(btn.Name, btn);
		}
	}
	public void AddButton(string id, BattleScreenButton button)
	{
		if (button == null) return;
		if (buttonList.Count > 0)
		{
			KeyValuePair<string, BattleScreenButton> lastButton = buttonList.Last();
			KeyValuePair<string, BattleScreenButton> firstButton = buttonList.First();
			firstButton.Value.ButtonFocusNeighborLeftId = id;
			lastButton.Value.ButtonFocusNeighborRightId = id;
			button.ButtonFocusNeighborRightId = firstButton.Key;
			button.ButtonFocusNeighborLeftId = lastButton.Key;
		}
		buttonList.Add(id, button);
	}

	public void ResetAllBattleButton()
	{
		foreach (KeyValuePair<string, BattleScreenButton> pair in buttonList)
		{
			pair.Value.Hover = false;
		}
	}

	public bool ResetBattleButton(string id)
	{
		if (GetBattleButton(id, out BattleScreenButton btn))
		{
			_prevButtonId = id;
			btn.Hover = false;
			return true;
		}
		return false;
	}

	public bool SetButtonHover(string id)
	{
		if (string.IsNullOrEmpty(id)) return false;
		if (buttonList.TryGetValue(id, out BattleScreenButton btn))
		{
			ResetBattleButton(_currentButtonId);
			_prevButtonId = _currentButtonId;
			_currentButtonId = id;
			btn.Hover = true;
			return true;
		}
		return false;
	}
	public void PressBattleButton(string id)
	{
		if (GetBattleButton(id, out BattleScreenButton btn))
		{
			btn.PressButton();
		}
	}

	public bool MoveButton(Vector2 dir)
	{
		bool successed = false;
		if (dir != Vector2.Zero)
		{
			int horizontal_move = Math.Sign(dir.X);
			int vertical_move = Math.Sign(dir.Y);

			GetBattleButton(_currentButtonId, out BattleScreenButton currentButton);
			if (horizontal_move != 0)
			{
				successed = successed || SetButtonHover(horizontal_move > 0 ?
					currentButton.ButtonFocusNeighborRightId : currentButton.ButtonFocusNeighborLeftId);
			}
			if (vertical_move != 0)
			{
				successed = successed || SetButtonHover(vertical_move > 0 ?
					currentButton.ButtonFocusNeighborDownId : currentButton.ButtonFocusNeighborUpId);
			}
		}
		return successed;

	}

	public bool GetBattleButton(string id, out BattleScreenButton button)
	{
		if (buttonList.TryGetValue(id, out button))
			return true;
		return false;
	}

	public bool GetBattleButtonId(int index, out string button)
	{
		KeyValuePair<string, BattleScreenButton> pair = buttonList.ElementAtOrDefault(index);
		button = pair.Key;
		if (pair.Equals(default(KeyValuePair<string, BattleScreenButton>)))
		{
			return true;
		}
		return false;
	}

	public string GetPrevHoverdBattleButtonId()
	{
		return _prevButtonId;
	}
	public string GetCurrentHoverdBattleButtonId()
	{
		return _currentButtonId;
	}

	public int GetBattleButtonCount()
	{
		return buttonList.Count;
	}
}
