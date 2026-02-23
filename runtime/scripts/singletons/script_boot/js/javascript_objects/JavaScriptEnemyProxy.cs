using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.Collections.Generic;
using System.Drawing;

public partial class JavaScriptEnemyProxy : BaseEnemy, IJavaScriptLifecyucle
{
    public ObjectInstance JsInstance
    {
        get => _jsInstance;
        set
        {
            _jsInstance = value;
            if (LifecycleProxy != null)
                LifecycleProxy.JsInstance = value;
        }
    }
    public string JsScriptPath { get; set; }
    public JavaScriptLifecycleProxy LifecycleProxy { get; set; } = new();
    private ObjectInstance _jsInstance = null;
    public override void _Ready()
    {
        base._Ready();
        AddChild(LifecycleProxy);
    }

    public override void _HandleAction(string action)
    {
        if (JsInstance?.HasProperty("onHandleAction") == true)
        {
            JsValue result = ((IJavaScriptObject)this).Invoke("onHandleAction", [action]);
            _AppendActionDialogue(result);
        }
    }

    private void _AppendActionDialogue(JsValue result)
    {
        if (result == null || result.IsNull() || result.IsUndefined())
            return;

        if (result.IsString())
        {
            string dialog = result.AsString();
            if (!string.IsNullOrEmpty(dialog))
                UtmxDialogueQueueManager.AppendDialogue(dialog);
            return;
        }

        if (result.IsArray())
        {
            var dialogArray = result.AsArray();
            int length = (int)dialogArray.Length;
            for (int i = 0; i < length; i++)
            {
                JsValue element = dialogArray.Get(i);
                if (element == null || element.IsNull() || element.IsUndefined())
                    continue;
                if (element.IsString())
                {
                    string dialogText = element.AsString();
                    if (!string.IsNullOrEmpty(dialogText))
                        UtmxDialogueQueueManager.AppendDialogue(dialogText);
                }
            }
            return;
        }

        object clrValue = result.ToObject();
        if (clrValue is string fallbackDialog && !string.IsNullOrEmpty(fallbackDialog))
        {
            UtmxDialogueQueueManager.AppendDialogue(fallbackDialog);
            return;
        }
        if (clrValue is object[] fallbackArray)
        {
            foreach (object element in fallbackArray)
            {
                if (element is string dialogText && !string.IsNullOrEmpty(dialogText))
                    UtmxDialogueQueueManager.AppendDialogue(dialogText);
            }
        }
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (JsInstance.HasProperty(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK))
            JavaScriptBridge.InvokeFunction(JsInstance, EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, [delta]);
    }
    public override void _OnSpare()
    {
        if (JsInstance?.HasProperty("onSpare") == true)
            ((IJavaScriptObject)this).Invoke("onSpare", []);
    }
    public override void _OnPlayerUsedItem()
    {
        if (JsInstance?.HasProperty("onPlayerUsedItem") == true)
            ((IJavaScriptObject)this).Invoke("onPlayerUsedItem", []);
    }
    public override void _OnDead()
    {
        if (JsInstance?.HasProperty("onDead") == true)
            ((IJavaScriptObject)this).Invoke("onDead", []);
    }
    public override void _OnDialogueStarting()
    {
        if (JsInstance?.HasProperty("onDialogueStarting") == true)
            ((IJavaScriptObject)this).Invoke("onDialogueStarting", []);
    }
    public override void _OnTurnStarting()
    {
        if (JsInstance?.HasProperty("onTurnStarting") == true)
            ((IJavaScriptObject)this).Invoke("onTurnStarting", []);
    }
    public override void _HandleAttack(UtmxBattleManager.AttackStatus status)
    {
        if (JsInstance?.HasProperty("onHandleAttack") == true)
            ((IJavaScriptObject)this).Invoke("onHandleAttack", [status]);
    }
    public override BaseBattleTurn _GetNextTurn()
    {
        if (JsInstance?.HasProperty("onGetNextTurn") == true)
        {
            JsValue result = ((IJavaScriptObject)this).Invoke("onGetNextTurn", []);
            if (result != null)
            {
                object JsObj = result.ToObject();
                if (JsObj is string)
                {
                    string path = JavaScriptModuleResolver.ResolvePath(JsScriptPath, result.ToString());
                    JavaScriptBattleTurnProxy battleTurn = IJavaScriptObject.New<JavaScriptBattleTurnProxy>(path);
                    if (battleTurn != null)
                        return battleTurn;
                }
                else if (JsObj is BaseBattleTurn)
                {
                    return IJavaScriptObject.Wrapper<JavaScriptBattleTurnProxy>(result);
                }
            }
        }
        return new BaseBattleTurn();
    }

    private Func<string, Dictionary<string, string>, bool> _CreateSpeechBubbleCmdCallback(JsValue processCmdCallback)
    {
        if (processCmdCallback == null || processCmdCallback.IsNull() || processCmdCallback.IsUndefined())
            return null;

        if (!processCmdCallback.IsObject())
            return null;

        JsValue callback = processCmdCallback;
        return (cmd, args) =>
        {
            try
            {
                JsValue result = JavaScriptBridge.MainEngine.Invoke(
                    callback,
                    JsValue.Undefined,
                    [
                        JsValue.FromObject(JavaScriptBridge.MainEngine, cmd ?? string.Empty),
                        JsValue.FromObject(JavaScriptBridge.MainEngine, args ?? new Dictionary<string, string>()),
                    ]
                );
                if (result == null || result.IsNull() || result.IsUndefined())
                    return false;
                return result.AsBoolean();
            }
            catch (Exception ex)
            {
                UtmxLogger.Error($"[SpeechBubbleCmd] {ex.Message}");
                return false;
            }
        };
    }

    public void AppendEnemyDialogue(object dialogueMessage, Vector2? offset = null, Vector2? size = null, JsValue processCmdCallback = null)
    {
        if (offset == null) offset = new Vector2(30, 0);
        if (size == null) size = new Vector2(180, 90);
        UtmxDialogueQueueManager.AppendBattleEnemyDialogue(
            EnemySlot,
            dialogueMessage.ToString(),
            (Vector2)offset,
            (Vector2)size,
            _CreateSpeechBubbleCmdCallback(processCmdCallback)
        );
    }
    public void AppendEnemyDialogue(object[] dialogueMessages, Vector2? offset = null, Vector2? size = null, JsValue processCmdCallback = null)
    {
        if (offset == null) offset = new Vector2(30, 0);
        if (size == null) size = new Vector2(180, 90);
        Func<string, Dictionary<string, string>, bool> callback = _CreateSpeechBubbleCmdCallback(processCmdCallback);
        foreach (string message in dialogueMessages)
        {
            UtmxDialogueQueueManager.AppendBattleEnemyDialogue(
                EnemySlot,
                message,
                (Vector2)offset,
                (Vector2)size,
                callback
            );
        }
    }
}
