using Godot;
using System;
using System.Collections.Generic;

public partial class UtmxBattleManager : Node
{
	public enum BattleCollisionLayers
	{
		Player = 1 << 1,
		Projectile = 1 << 2,
	};

	public string EncounterText { get => _encounterText; set => _encounterText = value; }
	public string FreeText { get => _freeText; set => _freeText = value; }
	public string DeathText { get => _deathText; set => _deathText = value; }
	public string EndText { get => _endText; set => _endText = value; }
	public bool CanFree { get => _canFree; set => _canFree = value; }
	public bool Endded { get => _endded; set => _endded = value; }
	public List<BaseEnemy> EnemysList { get => _enemysList; set => _enemysList = value; }
	public int TurnCounter { get => _turnCounter; set => _turnCounter = value; }
	public List<BattleTurn> CurrentTurnList { get => _currentTurnList; set => _currentTurnList = value; }
	public Vector2 PlayerSoulPosition { get => _playerSoulPosition; set => _playerSoulPosition = value; }
	public Color PlayerSoulColor { get => _playerSoulColor; set => _playerSoulColor = value; }

	public BattlePlayerSoul Soul => _playerSoul;
	public BattleMainArenaExpand MainArena => _mainArena;

	private bool _isInBattle;
	private StateMachine _battleStateMachine;
	private BattlePlayerSoul _playerSoul;
	private BattleMainArenaExpand _mainArena;
	private List<BaseEnemy> _enemysList = [];
	// 战斗场景中的实例

	private string _firstState = "";
	private string _encounterText = "";
	private string _freeText = "";
	private string _deathText = "";
	private string _endText = "";
	private bool _canFree = true;
	private bool _endded = false;

	private int _turnCounter = 0;
	private double _turnTimer = 0.0;
	public BaseEncounterConfiguration _encounterConfig = new();
	private List<BattleTurn> _currentTurnList = new() { new BattleTurn() };
	private Vector2 _playerSoulPosition = Vector2.Zero;
	private Color _playerSoulColor = Colors.Red;

	private static readonly Lazy<UtmxBattleManager> _instance =
		new Lazy<UtmxBattleManager>(() => new UtmxBattleManager());
	private UtmxBattleManager()
	{
	}
	public static UtmxBattleManager Instance => _instance.Value;

	public void EncounterBattleStart(BaseEncounterConfiguration configuration)
	{
		UtmxDialogueQueueManager.Instance.ClearDialogue();

		_encounterText = configuration.EncounterText;
		_freeText = configuration.FreeText;
		_deathText = configuration.DeathText;
		_endText = configuration.EndText;
		_canFree = configuration.CanFree;
		_firstState = configuration.EncounterBattleFirstState;

		foreach (BaseEnemy enemy in _enemysList) enemy.QueueFree();
		_enemysList.Clear();
		foreach (string enemyId in configuration.EnemysList)
		{
			if (GameRegisterDB.TryGetEnemy(enemyId, out BaseEnemy enemy))
			{
				_enemysList.Add(enemy);
			}
		}
		UtmxSceneManager.Instance.ChangeSceneToFile(UtmxSceneManager.Instance.EncounterBattleScenePath);
	}
	public void EncounterBattleEnd()
	{
		UninitializeBattle();
	}

	public void InitializeBattle(StateMachine stateMachine, BattlePlayerSoul soul, BattleMainArenaExpand mainArena)
	{
		_battleStateMachine = stateMachine;
		_playerSoul = soul;
		_mainArena = mainArena;
		_isInBattle = true;
	}

	public void UninitializeBattle()
	{
		if (_isInBattle)
		{
			_battleStateMachine = null;
			_playerSoul = null;
			_mainArena = null;
			_isInBattle = false;
		}
	}
	public void GameOver()
	{
		if (_isInBattle)
		{
			EnemysList.Clear();
			Camera2D camera = _playerSoul.GetViewport().GetCamera2D();
			PlayerSoulPosition = camera.GetCanvasTransform().BasisXform(_playerSoul.GlobalPosition);
			PlayerSoulColor = _playerSoul.Modulate;
			UtmxSceneManager.Instance.ChangeSceneToFile(UtmxSceneManager.Instance.GameoverScenePath);
		}
	}



	public bool IsInBattle() { return _isInBattle; }
	public BattlePlayerSoul GetPlayerSoul() { return _playerSoul; }
	public BattleMainArenaExpand GetMainArena() { return _mainArena; }
	public int GetEnemysCount() { return EnemysList.Count; }

	public void SwitchBattleState(string stateId)
	{
		_battleStateMachine.SwitchToState(stateId);
	}
	public string GetFirstBattleState()
	{
		return _firstState;
	}


	#region 回合管理

	public void TurnInitialize()
	{
		_currentTurnList.Clear();
		foreach (BaseEnemy enemy in _enemysList)
		{
			_currentTurnList.Add(enemy._GetNextTurn());
		}
		foreach (BattleTurn turn in _currentTurnList)
		{
			turn._OnTurnInitialize();
		}
	}
	public bool TurnStart()
	{
		if (_currentTurnList.Count > 0)
		{
			foreach (BattleTurn turn in _currentTurnList)
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
			foreach (BattleTurn turn in _currentTurnList)
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
			foreach (BattleTurn turn in _currentTurnList)
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
			return MainArena.GlobalPosition - new Vector2(0F, MainArena.Size.Y * 0.5F);
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
			return MainArena.Size;
		}
	}
	#endregion
}
