using Godot;
using System;
using System.Collections.Generic;

public partial class UtmxBattleManager
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

	public static bool Endded { get => _endded; set => _endded = value; }
	public static Vector2 PlayerSoulPosition { get => _playerSoulPosition; set => _playerSoulPosition = value; }
	public static Color PlayerSoulColor { get => _playerSoulColor; set => _playerSoulColor = value; }

	private static BaseEncounter _battleEncounter;
	private static BattleController _battleController;
	private static bool _endded = false;
	private static bool _isInBattle;
	private static Vector2 _playerSoulPosition = Vector2.Zero;
	private static Color _playerSoulColor = Colors.Red;

	public static void EncounterBattleStart(string encounterId)
	{
		if (UtmxGameRegisterDB.TryGetEncounter(encounterId, out BaseEncounter encounter))
		{
			_battleEncounter = encounter;
			Endded = false;
			UtmxSceneManager.Instance.ChangeSceneToFile(UtmxSceneManager.Instance.EncounterBattleScenePath);
		}
		else
		{
			UtmxLogger.Error($"{TranslationServer.Translate("Failed to start battle: invalid encounter")}: {encounterId}");
		}
	}
	public static void EncounterBattleEnd()
	{
		Endded = true;
		if (IsInBattle())
		{
			GetEncounterInstance()._OnBattleEnd();
			_battleEncounter = null;
			_battleController = null;
			_isInBattle = false;
		}
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
			BattlePlayerSoul soul = GetBattlePlayerController().PlayerSoul;
			Camera2D camera = soul.GetViewport().GetCamera2D();
			PlayerSoulPosition = camera.GetCanvasTransform().BasisXform(soul.GlobalPosition);
			PlayerSoulColor = soul.Modulate;
			EncounterBattleEnd();
			UtmxSceneManager.Instance.CallDeferred("ChangeSceneToFile", [UtmxSceneManager.Instance.GameoverScenePath]);
		}
	}


	public static bool IsInBattle() { return _isInBattle; }
	public static BaseEncounter GetEncounterInstance() { return _battleEncounter; }
	public static BattleController GetBattleController() { return _battleController; }
	public static BattleEnemyController GetBattleEnemyController() { 
		return _battleController.EnemyController;
	}
	public static BattleTurnController GetBattleTurnController()
	{
		return _battleController.TurnController;
	}
	public static BattlePlayerController GetBattlePlayerController()
	{
		return _battleController.PlayerController;
	}
	public static BattleArenaController GetBattleArenaController()
	{
		return _battleController.ArenaController;
	}
	public static BattleProjectileController GetBattleProjectileController()
	{
		return _battleController.ProjectileController;
	}

	public static void SwitchBattleState(string stateId)
	{
		_battleController.SwitchToState(stateId);
	}

	public static void ShowDialogueText(object texts)
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
