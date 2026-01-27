using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalBattleManager : Node
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
	public List<BattleTurn> TurnList { get => _turnList; set => _turnList = value; }
	public int TurnCounter { get => _turnCounter; set => _turnCounter = value; }
	public BattleTurn CurrentTurn { get => _currentTurn; set => _currentTurn = value; }
	public Vector2 PlayerSoulPosition { get => _playerSoulPosition; set => _playerSoulPosition = value; }
	public Color PlayerSoulColor { get => _playerSoulColor; set => _playerSoulColor = value; }

	public BattlePlayerSoul Soul => _playerSoul;
	public BattleMainArenaExpand MainArena => _mainArena;

	public EncounterConfiguration Config
	{
		get => _encounterConfig;
		set
		{
			_encounterConfig = value;
			_canFree = _encounterConfig.CanFree;
			_encounterText = _encounterConfig.EncounterText;
			_freeText = _encounterConfig.FreeText;
			_deathText = _encounterConfig.DeathText;
			_endText = _encounterConfig.EndText;
		}
	}

	private bool _isInBattle;
	public EncounterConfiguration _encounterConfig = new EncounterConfiguration();
	private StateMachine _battleStateMachine;
	private BattlePlayerSoul _playerSoul;
	private BattleMainArenaExpand _mainArena;
	private string _encounterText = "";
	private string _freeText = "";
	private string _deathText = "";
	private string _endText = "";
	private bool _canFree = true;
	private bool _endded = false;
	private int _turnCounter = 0;
	private BattleTurn _currentTurn = new BattleTurn();
	private List<BaseEnemy> _enemysList = [
		new BaseEnemy()
	];
	private List<BattleTurn> _turnList = [
	];
	private Vector2 _playerSoulPosition = Vector2.Zero;
	private Color _playerSoulColor = Colors.Red;

	private static readonly Lazy<GlobalBattleManager> _instance =
		new Lazy<GlobalBattleManager>(() => new GlobalBattleManager());
	private GlobalBattleManager()
	{
		Config = new EncounterConfiguration();
	}
	public static GlobalBattleManager Instance => _instance.Value;

	public void EncounterBattleStart()
	{
		SceneManager.Instance.ChangeSceneToFile(SceneManager.Instance.EncounterBattleScenePath);
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
		_battleStateMachine = null;
		_playerSoul = null;
		_mainArena = null;
		_isInBattle = false;
	}
	public void GameOver()
	{
		if (_isInBattle)
		{
			EnemysList.Clear();
			Camera2D camera = _playerSoul.GetViewport().GetCamera2D();
			PlayerSoulPosition = camera.GetCanvasTransform().BasisXform(_playerSoul.GlobalPosition);
			PlayerSoulColor = _playerSoul.Modulate;
			SceneManager.Instance.ChangeSceneToFile(SceneManager.Instance.GameoverScenePath);
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

	public void NextTurn()
	{
		if (GlobalBattleManager.Instance.TurnList.Count > _turnCounter)
		{
			_currentTurn = GlobalBattleManager.Instance.TurnList[_turnCounter];
			_turnCounter += 1;
		}
	}
	public BattleTurn GetCurrentTurn()
	{
		return _currentTurn;
	}
}
