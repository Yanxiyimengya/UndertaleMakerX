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
				if (! bubble.IsTextTyperFinished())
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
		BattleMainArenaExpand _battleMainArena = GlobalBattleManager.Instance.GetMainArena();
		BattleTurn currentTurn = GlobalBattleManager.Instance.GetCurrentTurn();
		BattlePlayerSoul soul = GlobalBattleManager.Instance.GetPlayerSoul();
		currentTurn.Initialize();
		soul.GlobalPosition = currentTurn.SoulPosition;
		soul.Movable = false;

		if (_tween != null && _tween.IsRunning())
		{
			_tween.Kill();
		}
		_tween = GetTree().CreateTween();
		_tween.TweenProperty(_battleMainArena, "Size", currentTurn.ArenaSize, 0.4);
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


		if (DialogueQueueManager.Instance.BattleEnemyDialogueCount() > 0)
		{
			Dictionary<int ,DialogueQueueManager.Dialogue> dialogue = DialogueQueueManager.Instance.GetBattleEnemyDialogues();
			if (DialogueSpeechBubblePackedScene != null)
			{
				foreach (KeyValuePair<int, DialogueQueueManager.Dialogue> pair in dialogue)
				{
					if (pair.Key >= 0 && pair.Key < GlobalBattleManager.Instance.GetEnemysCount())
					{
						BaseEnemy enemy = GlobalBattleManager.Instance.EnemysList[pair.Key];
						Node inst = DialogueSpeechBubblePackedScene.Instantiate();
						if (inst is SpeechBubble bubble)
						{
							if (! enemy.IsInsideTree()) continue;
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
			SwitchState("BattleEnemyState");
	}
}
