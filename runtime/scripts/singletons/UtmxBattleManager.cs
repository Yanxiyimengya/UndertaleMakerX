using Godot;
using System;
using System.Collections.Generic;

public partial class UtmxBattleManager
{
    public enum BattleCollisionLayers
    {
        Arena = 0b10,
        Player = 0b100,
        Projectile = 0b1000,
    };
    public enum BattleStatus
    {
        Player = 0,
        PlayerDialogue = 1,
        EnemyDialogue = 2,
        Enemy = 3,
        End = 4,
    };
    public enum AttackStatus
    {
        Selected = 0,  // 菜单中被选中
        Hit = 1,     // 确认攻击
        Missed = 2      // 未确认
    };

    public static bool Endded { get => _endded; set => _endded = value; }
    public static Transform2D PlayerSoulTransform { get => _playerSoulTransform; set => _playerSoulTransform = value; }
    public static Color PlayerSoulColor { get => _playerSoulColor; set => _playerSoulColor = value; }
    public static string PrevScenePath { get => _prevScenePath; set => _prevScenePath = value; }

    private static BaseEncounter _battleEncounter;
    private static BattleController _battleController;
    private static bool _endded = false;
    private static bool _isInBattle;
    private static Transform2D _playerSoulTransform;
    private static Color _playerSoulColor = Colors.Red;
    private static string _prevScenePath = "";

    public static bool StartEncounterBattle(string encounterId)
    {
        if (UtmxGameRegisterDB.TryGetEncounter(encounterId, out BaseEncounter encounter))
        {
            if (_isInBattle) EndEncounterBattle();
            _battleEncounter = encounter;
            Endded = false;
            PrevScenePath = UtmxSceneManager.GetCurrentScenePath();
            UtmxSceneManager.ChangeSceneToFile(UtmxSceneManager.Instance.EncounterBattleScenePath);
            return true;
        }
        else
        {
            UtmxLogger.Error($"{TranslationServer.Translate("Failed to start battle: invalid encounter")}: {encounterId}");
        }
        return false;
    }
    public static void EndEncounterBattle()
    {
        UtmxSceneManager.ChangeSceneToFile(UtmxBattleManager.PrevScenePath);
        Endded = true;
        _battleEncounter = null;
        _battleController = null;
        _isInBattle = false;
        GetEncounterInstance()?._OnBattleEnd();
    }

    public static void InitializeBattle(BattleController battleController)
    {
        _battleController = battleController;
        _isInBattle = true;
    }

    public static void GameOver()
    {
        if (_isInBattle)
        {
            UtmxSceneManager.Instance.CallDeferred("ChangeSceneToFile", [UtmxSceneManager.Instance.GameoverScenePath]);
            _isInBattle = false;
            BattlePlayerSoul soul = GetBattlePlayerController().PlayerSoul;
            Camera2D camera = soul.GetViewport().GetCamera2D();
            _playerSoulTransform = soul.Sprite.GlobalTransform;
            PlayerSoulColor = soul.SoulColor;
        }
    }

    public static bool IsInBattle() { return _isInBattle; }
    public static BaseEncounter GetEncounterInstance() { return _battleEncounter; }
    public static BattleController GetBattleController() { return _battleController; }
    public static BattleEnemyController GetBattleEnemyController()
    {
        return _battleController?.EnemyController;
    }
    public static BattleTurnController GetBattleTurnController()
    {
        return _battleController?.TurnController;
    }
    public static BattlePlayerController GetBattlePlayerController()
    {
        return _battleController?.PlayerController;
    }
    public static BattleArenaController GetBattleArenaController()
    {
        return _battleController?.ArenaController;
    }
    public static BattleProjectileController GetBattleProjectileController()
    {
        return _battleController?.ProjectileController;
    }
    public static BattleUiController GetBattleUiController()
    {
        return _battleController?.UiController;
    }

    public static void SwitchBattleState(string stateId)
    {
        _battleController?.SwitchToState(stateId);
    }
    public static void SwitchStatus(BattleStatus status)
    {
        _battleController?.SwitchStatus(status);
    }
    public static void SwitchStatus(int status)
    {
        if (!Enum.IsDefined(typeof(BattleStatus), status))
        {
            UtmxLogger.Error($"Failed to switch battle status: invalid status value {status}");
            return;
        }
        SwitchStatus((BattleStatus)status);
    }
    public static void SwitchStatus(double status)
    {
        if (double.IsNaN(status) || double.IsInfinity(status))
        {
            UtmxLogger.Error($"Failed to switch battle status: invalid number {status}");
            return;
        }

        int statusInt = (int)status;
        if (Math.Abs(status - statusInt) > double.Epsilon)
        {
            UtmxLogger.Error($"Failed to switch battle status: status must be an integer, got {status}");
            return;
        }
        SwitchStatus(statusInt);
    }
    public static void SwitchStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            UtmxLogger.Error("Failed to switch battle status: status is empty");
            return;
        }

        if (Enum.TryParse<BattleStatus>(status, true, out BattleStatus parsed))
        {
            SwitchStatus(parsed);
            return;
        }

        string normalized = status.Replace("_", string.Empty).Replace(" ", string.Empty);
        foreach (BattleStatus value in Enum.GetValues(typeof(BattleStatus)))
        {
            if (string.Equals(value.ToString(), normalized, StringComparison.OrdinalIgnoreCase))
            {
                SwitchStatus(value);
                return;
            }
        }

        UtmxLogger.Error($"Failed to switch battle status: invalid status '{status}'");
    }

    public static void ShowDialogueText(object texts)
    {
        if (texts != null)
        {
            if (texts is string dialog && !string.IsNullOrEmpty(dialog))
            {
                UtmxDialogueQueueManager.AppendDialogue(dialog);
            }
            else if (texts is object[] dialogArray)
            {
                foreach (object elements in dialogArray)
                {
                    if (elements is string dialogText && !string.IsNullOrEmpty(dialogText))
                    {
                        UtmxDialogueQueueManager.AppendDialogue(dialogText);
                    }
                }
            }
        }
    }
}
