using Godot;
using static Godot.HttpRequest;

public partial class JavaScriptEnemyProxy : BaseEnemy
{
    public JavaScriptObjectInstance JsInstance;
    public override void _OnSpare()
    {
        JsInstance.Invoke("onSpare", []);
    }

    public override void _HandleAction(string action)
    {
        object result = JsInstance.Invoke("onHandleAction", [action]);
        if (result != null)
        { 
            if (result is string dialog && !string.IsNullOrEmpty(dialog))
            {
                UtmxDialogueQueueManager.Instance.AppendDialogue(dialog);
            }
            else if (result is object[] dialogArray)
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
    public override void _HandleAttack(AttackStatus status)
    {
        JsInstance.Invoke("onHandleAttack", [status]);
    }
    public override BattleTurn _GetNextTurn()
    {
        object result = JsInstance.Invoke("onGetNextTurn", []);
        if (result != null && result is string path && !string.IsNullOrEmpty(path))
        {
            JavaScriptClass jsClass = JavaScriptBridge.FromFile(path);
            if (jsClass != null)
            {
                JavaScriptObjectInstance instance = jsClass.New();
                BattleTurn battleTurn = instance.ToObject() as BattleTurn;
                if (battleTurn != null)
                {
                    return battleTurn;
                }
            }
        }
        return new BattleTurn();
    }

    public void AppendEnemyDialogue(object dialogueMessage, Vector2? offset = null, bool hideSpike = false, int dir = 2)
    {
        if (offset == null)
            offset = new Vector2(30, 0);
        UtmxDialogueQueueManager.Instance.AppendBattleEnemyDialogue(EnemySlot, dialogueMessage.ToString(), (Vector2)offset, hideSpike);
    }
    public void AppendEnemyDialogue(object[] dialogueMessages, Vector2? offset = null, bool hideSpike = false, int dir = 2)
    {
        if (offset == null)
            offset = new Vector2(30, 0);
        foreach (string message in dialogueMessages)
        {
            UtmxDialogueQueueManager.Instance.AppendBattleEnemyDialogue(EnemySlot, message, (Vector2)offset, hideSpike);
        }
    }
}
