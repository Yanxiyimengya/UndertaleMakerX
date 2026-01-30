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

	private Tween _tween;
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

	public override void _EnterState()
	{
		MenuManager.CloseAllMenu();
		BattleMainArenaExpand _battleMainArena = UtmxBattleManager.Instance.GetMainArena();
        UtmxBattleManager.Instance.TurnInitialize();
		BattlePlayerSoul soul = UtmxBattleManager.Instance.GetPlayerSoul();
		soul.GlobalPosition = UtmxBattleManager.Instance.GetTurnSoulInitializePosition();
		soul.Movable = false;
		soul.Visible = true;
        if (_tween != null && _tween.IsRunning())
		{
			_tween.Kill();
		}
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(_battleMainArena, "Size", UtmxBattleManager.Instance.GetTurnArenaInitializeSize(), 0.4);
		NextStep();
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


		if (UtmxDialogueQueueManager.Instance.BattleEnemyDialogueCount() > 0)
		{
			Dictionary<int, UtmxDialogueData> dialogue = UtmxDialogueQueueManager.Instance.GetBattleEnemyDialogues();
			if (DialogueSpeechBubblePackedScene != null)
			{
				foreach (KeyValuePair<int, UtmxDialogueData> pair in dialogue)
				{
					if (pair.Key >= 0 && pair.Key < UtmxBattleManager.Instance.GetEnemysCount())
					{
						BaseEnemy enemy = UtmxBattleManager.Instance.EnemysList[pair.Key];
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
			if (UtmxBattleManager.Instance.GetTurnCount() > 0)
			{
				SwitchState("BattleEnemyState");
			}
			else
            {
                SwitchState("BattlePlayerChoiceActionState");
            }
		}
	}
}
