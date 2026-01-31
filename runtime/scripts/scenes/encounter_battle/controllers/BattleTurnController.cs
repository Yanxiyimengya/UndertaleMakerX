using Godot;
using System;
using System.Collections.Generic;

public partial class BattleTurnController : Node
{
	public int TurnCounter { get => _turnCounter; set => _turnCounter = value; }
	public List<BaseBattleTurn> CurrentTurnList { get => _currentTurnList; set => _currentTurnList = value; }

	private int _turnCounter = 0;
	private double _turnTimer = 0.0;
	private List<BaseBattleTurn> _currentTurnList = new();
	public void TurnInitialize()
	{
		_currentTurnList.Clear();
		foreach (BaseEnemy enemy in UtmxBattleManager.Instance.GetBattleEnemyController().EnemyList)
		{
			_currentTurnList.Add(enemy._GetNextTurn());
		}
		foreach (BaseBattleTurn turn in _currentTurnList)
		{
			turn._OnTurnInitialize();
		}
	}
	public bool TurnStart()
	{
		if (_currentTurnList.Count > 0)
		{
			foreach (BaseBattleTurn turn in _currentTurnList)
			{
				turn._OnTurnStart();
				_turnTimer = Math.Max(_turnTimer, turn.TurnTime);
			}
			return true;
		}
		return false;
	}
	public bool TurnUpdate(double delta)
	{
		if (_currentTurnList.Count > 0)
		{
			if (_turnTimer <= 0.0)
			{
				return false;
			}
			else
			{
				_turnTimer -= delta;
			}
			foreach (BaseBattleTurn turn in _currentTurnList)
			{
				turn._OnTurnUpdate(delta);
			}
			return true;
		}
		return false;
	}
	public bool TurnEnd()
	{
		if (_currentTurnList.Count > 0)
		{
			foreach (BaseBattleTurn turn in _currentTurnList)
			{
				turn._OnTurnEnd();
			}
			return true;
		}
		return false;
	}
	public int GetTurnCount()
	{
		return _currentTurnList.Count;
	}
	public Vector2 GetTurnSoulInitializePosition()
	{
		if (_currentTurnList.Count > 0)
		{
			return _currentTurnList[0].SoulInitializePosition;
		}
		else
		{
			BattleMainArenaExpand mainArena = UtmxBattleManager.Instance.GetBattleController().MainArena;
			return mainArena.GlobalPosition - new Vector2(0F, mainArena.Size.Y * 0.5F);
		}
	}
	public Vector2 GetTurnArenaInitializeSize()
	{
		if (_currentTurnList.Count > 0)
		{
			return _currentTurnList[0].ArenaInitializeSize;
		}
		else
		{
			BattleMainArenaExpand mainArena = UtmxBattleManager.Instance.GetBattleController().MainArena;
			return mainArena.Size;
		}
	}
}
