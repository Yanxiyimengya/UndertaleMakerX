using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Node
{
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

	public EncounterBattle Battle => _encounterBattle;
	public BattlePlayerSoul Soul => _playerSoul;
	public BattleMainArenaExpand MainArena => _mainArena;

	public EncounterConfiguration Config
	{
		get => _encounterConfig;
		set
		{
			_encounterConfig = value;
			_canFree = _encounterConfig.CanFree;
			_encounterText = _encounterConfig.DefaultEncounterText;
			_freeText = _encounterConfig.FreeText;
			_deathText = _encounterConfig.DeathText;
			_endText = _encounterConfig.EndText;
		}
	}

	private bool _isInBattle;
	public EncounterConfiguration _encounterConfig = new EncounterConfiguration();
	private EncounterBattle _encounterBattle;
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

	private static readonly Lazy<BattleManager> _instance =
		new Lazy<BattleManager>(() => new BattleManager());
	private BattleManager()
	{
		Config = new EncounterConfiguration();
	}
	public static BattleManager Instance => _instance.Value;

	public void EncounterBattleStart()
	{

	}

	public void InitializeBattle(EncounterBattle battleRoot, BattlePlayerSoul soul, BattleMainArenaExpand mainArena)
	{
		_encounterBattle = battleRoot;
		_playerSoul = soul;
		_mainArena = mainArena;
		_isInBattle = true;
	}

	public void UninitializeBattle()
	{
		_encounterBattle = null;
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
		_encounterBattle.BattleStateMachine.SwitchToState(stateId);
	}

	public void NextTurn()
	{
		if (BattleManager.Instance.TurnList.Count > _turnCounter)
		{
			_currentTurn = BattleManager.Instance.TurnList[_turnCounter];
			_turnCounter += 1;
		}
	}
	public BattleTurn GetCurrentTurn()
	{
		return _currentTurn;
	}
}
