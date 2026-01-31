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
    public enum BattleStatus
    {
        Player,
        PlayerDialogue,
        EnemyDialogue,
        Enemy,
    }

    public bool Endded { get => _endded; set => _endded = value; }
    public Vector2 PlayerSoulPosition { get => _playerSoulPosition; set => _playerSoulPosition = value; }
    public Color PlayerSoulColor { get => _playerSoulColor; set => _playerSoulColor = value; }

    private BaseEncounter _battleEncounter;
    private BattleController _battleController;
    private bool _endded = false;
    private static bool _isInBattle;
    private Vector2 _playerSoulPosition = Vector2.Zero;
    private Color _playerSoulColor = Colors.Red;

    private static readonly Lazy<UtmxBattleManager> _instance =
        new Lazy<UtmxBattleManager>(() => new UtmxBattleManager());
    private UtmxBattleManager()
    {
    }
    public static UtmxBattleManager Instance => _instance.Value;

    public static void EncounterBattleStart(string encounterId)
    {
        if (GameRegisterDB.TryGetEncounter(encounterId, out BaseEncounter encounter))
        {
            UtmxBattleManager.Instance._battleEncounter = encounter;
            UtmxBattleManager.Instance.Endded = false;
            UtmxSceneManager.Instance.ChangeSceneToFile(UtmxSceneManager.Instance.EncounterBattleScenePath);
        }
        else
        {
            UtmxLogger.Error($"{TranslationServer.Translate("Failed to start battle: invalid encounter")}: {encounterId}");
        }
    }
    public static void EncounterBattleEnd()
    {
        UtmxBattleManager.Instance.Endded = true;
        if (IsInBattle())
        {
            UtmxBattleManager.Instance.GetEncounterInstance()._OnBattleEnd();
            UtmxBattleManager.Instance._battleEncounter = null;
            UtmxBattleManager.Instance._battleController = null;
            _isInBattle = false;
        }
    }

    public void InitializeBattle(BattleController battleController)
    {
        _battleController = battleController;
        _isInBattle = true;
    }

    public void GameOver()
    {
        if (_isInBattle)
        {
            EncounterBattleEnd();
            Camera2D camera = _battleController.PlayerSoul.GetViewport().GetCamera2D();
            PlayerSoulPosition = camera.GetCanvasTransform().BasisXform(_battleController.PlayerSoul.GlobalPosition);
            PlayerSoulColor = _battleController.PlayerSoul.Modulate;
            UtmxSceneManager.Instance.ChangeSceneToFile(UtmxSceneManager.Instance.GameoverScenePath);
        }
    }


    public static bool IsInBattle() { return _isInBattle; }
    public BaseEncounter GetEncounterInstance() { return _battleEncounter; }
    public BattleController GetBattleController() { return _battleController; }
    public BattleEnemyController GetBattleEnemyController() { 
        return _battleController.EnemyController;
    }
    public BattleTurnController GetBattleTurnController()
    {
        return _battleController.TurnController;
    }
    public BattlePlayerController GetBattlePlayerController()
    {
        return _battleController.PlayerController;
    }
    public BattleProjectileController GetBattleProjectileController()
    {
        return _battleController.ProjectileController;
    }

    public void SwitchBattleState(string stateId)
    {
        _battleController.SwitchToState(stateId);
    }

    public void ShowDialogueText(object texts)
    {
        if (texts != null)
        {
            if (texts is string dialog && !string.IsNullOrEmpty(dialog))
            {
                UtmxDialogueQueueManager.Instance.AppendDialogue(dialog);
            }
            else if (texts is object[] dialogArray)
            {
                foreach (object elements in dialogArray)
                {
                    if (elements is string dialogText && !string.IsNullOrEmpty(dialogText))
                    {
                        UtmxDialogueQueueManager.Instance.AppendDialogue(dialogText);
                    }
                }
            }
        }
    }
}
