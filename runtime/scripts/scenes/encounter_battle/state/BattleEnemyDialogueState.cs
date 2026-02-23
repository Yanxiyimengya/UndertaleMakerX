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
    [Export]
    BattleScreenButtonManager BattleButtonManager;

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
        BattleButtonManager.ResetAllBattleButton();
        SetProcess(false);
        MenuManager.CloseAllMenu();
        UtmxBattleManager.GetBattleTurnController().TurnInitialize();
        BattlePlayerSoul soul = UtmxBattleManager.GetBattlePlayerController().PlayerSoul;
        soul.GlobalPosition = UtmxBattleManager.GetBattleTurnController().GetTurnSoulInitializePosition();
        soul.Movable = false;
        soul.Visible = true;

        BattleMainArenaExpand _battleMainArena = UtmxBattleManager.GetBattleArenaController().MainArena;
        Vector2 initSize = UtmxBattleManager.GetBattleTurnController().GetTurnarenaInitSize();
        if (_battleMainArena.Size != initSize)
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
            if (GodotObject.IsInstanceValid(bubble))
                bubble.Destroy();
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
                            pair.Value.TryGetMetaData("Poisition", out Variant offset);
                            bubble.Position = enemy.CenterPosition + offset.AsVector2();
                            if (pair.Value.TryGetMetaData("Dir", out Variant dir))
                                bubble.Dir = dir.AsInt32();
                            if (pair.Value.TryGetMetaData("HideSpike", out Variant hideSpike))
                                bubble.HideSpike = hideSpike.AsBool();
                            if (pair.Value.TryGetMetaData("Size", out Variant size))
                                bubble.Size = size.AsVector2();

                            enemy.AddChild(bubble);
                            if (GodotObject.IsInstanceValid(bubble.SpeechBubbleTextTyper))
                            {
                                bubble.SpeechBubbleTextTyper.ProcessCmdCallback = pair.Value.ProcessCmdCallback;
                            }
                            bubble.Text = pair.Value.Message;
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
