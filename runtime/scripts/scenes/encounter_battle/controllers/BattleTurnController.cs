using Godot;
using System;
using System.Collections.Generic;

public partial class BattleTurnController : Node
{
	public List<BaseBattleTurn> CurrentTurnList { get => _currentTurnList; set => _currentTurnList = value; }

	private double _turnTimer = 0.0;
	private bool _turnStart = false;
	private List<BaseBattleTurn> _currentTurnList = new();

	public override void _Process(double delta)
	{
		if (_turnStart)
		{
			if (! TurnUpdate(delta))
			{
				TurnEnd();
			}
		}
	}
	public bool IsTurnInProgress()
	{
		return _turnStart;
	}
	public void TurnInitialize()
	{
		_currentTurnList.Clear();
		foreach (BaseEnemy enemy in UtmxBattleManager.GetBattleEnemyController().EnemiesList)
		{
			_currentTurnList.Add(enemy._GetNextTurn());
		}
		foreach (BaseBattleTurn turn in _currentTurnList)
		{
			turn._OnTurnInit();
		}
	}
	public bool TurnStart()
	{
		if (_currentTurnList.Count > 0)
		{
			_turnStart = true;
			_turnTimer = 0.0;
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
		_turnStart = false;
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
		BattleMainArenaExpand mainArena = UtmxBattleManager.GetBattleArenaController().MainArena;
		Vector2 soulPosition = mainArena.GlobalPosition - new Vector2(0F, mainArena.Size.Y * 0.5F);

		if (_currentTurnList.Count == 0)
			return soulPosition;

		return soulPosition + _currentTurnList[0].SoulInitializePosition;
	}
	public Vector2 GetTurnarenaInitSize()
	{
		if (_currentTurnList.Count > 0)
		{
			return _currentTurnList[0].arenaInitSize;
		}
		else
		{
			BattleMainArenaExpand mainArena = UtmxBattleManager.GetBattleArenaController().MainArena;
			return mainArena.Size;
		}
	}
}
