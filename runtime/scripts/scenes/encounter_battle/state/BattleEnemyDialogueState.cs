using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BattleEnemyDialogueState : StateNode
{
	[Export]
	public PackedScene DialogueSpeechBubblePackedScene;
	[Export]
	public BattleMenuManager MenuManager;

	private List<SpeechBubble> _speechBubbleList = new();
	public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("confirm"))
		{
			bool allFinished = true;
			foreach (SpeechBubble bubble in _speechBubbleList)
			{
				if (!bubble.IsTextTyperFinished())
				{
					allFinished = false;
				}
			}
			if (allFinished)
			{
				NextStep();
			}
		}
	}

	public async override void _EnterState()
    {
        SetProcess(false);
        MenuManager.CloseAllMenu();
		UtmxBattleManager.GetBattleTurnController().TurnInitialize();
		BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
		soul.GlobalPosition = UtmxBattleManager.GetBattleTurnController().GetTurnSoulInitializePosition();
		soul.Movable = false;
		soul.Visible = true;
        BattleMainArenaExpand _battleMainArena = UtmxBattleManager.GetBattleArenaController().MainArena;
        await _battleMainArena.Resize(UtmxBattleManager.GetBattleTurnController().GetTurnarenaInitSize(), 0.4);
        foreach (BaseEnemy enemy in UtmxBattleManager.GetBattleEnemyController().EnemiesList)
        {
            enemy._OnDialogueStarting();
        }
        NextStep();
        SetProcess(true);
    }

	public override void _ExitState()
    {
    }


	private void NextStep()
	{
		foreach (SpeechBubble bubble in _speechBubbleList)
		{
			bubble.QueueFree();
		}
		_speechBubbleList.Clear();


		if (UtmxDialogueQueueManager.BattleEnemyDialogueCount() > 0)
		{
			Dictionary<int, UtmxDialogueData> dialogue = UtmxDialogueQueueManager.GetBattleEnemyDialogues();
			if (DialogueSpeechBubblePackedScene != null)
			{
				foreach (KeyValuePair<int, UtmxDialogueData> pair in dialogue)
				{
					if (pair.Key >= 0 && pair.Key < UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount())
					{
						BaseEnemy enemy = UtmxBattleManager.GetBattleEnemyController().EnemiesList[pair.Key];
						Node inst = DialogueSpeechBubblePackedScene.Instantiate();
						if (inst is SpeechBubble bubble)
						{
							if (!enemy.IsInsideTree()) continue;
							enemy.AddChild(bubble);
							bubble.Text = pair.Value.Message;
							pair.Value.TryGetMetaData("Poisition", out Variant offset);
							pair.Value.TryGetMetaData("HideSpike", out Variant hideSpike);
							pair.Value.TryGetMetaData("Dir", out Variant dir);
							bubble.Position = enemy.CenterPosition + offset.AsVector2();
							bubble.Dir = dir.AsInt32();
							bubble.HideSpike = hideSpike.AsBool();
							_speechBubbleList.Add(bubble);
						}
						else
						{
							inst.Free();
						}
					}
				}
			}
		}
		else
		{
			if (UtmxBattleManager.GetBattleTurnController().GetTurnCount() > 0)
			{
				UtmxBattleManager.GetBattleController().ChangeToEnemyTurnState();
			}
			else
			{
				UtmxBattleManager.GetBattleController().ChangeToPlayerTurnState();
			}
		}
	}
}
