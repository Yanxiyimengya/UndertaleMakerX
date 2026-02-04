using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System;

public partial class JavaScriptEnemyProxy : BaseEnemy, IJavaScriptObject
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }

	public override void _HandleAction(string action)
	{
		object result = ((IJavaScriptObject)this).Invoke("onHandleAction", [action]);
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

	public override void _Process(double delta)
	{
		if (JsInstance.HasProperty(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK))
			JavaScriptBridge.InvokeFunction(JsInstance, EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, [delta]);
    }
    public override void _OnSpare()
    {
        ((IJavaScriptObject)this).Invoke("onSpare", []);
    }

    public override void _OnBattleStart()
    {
        ((IJavaScriptObject)this).Invoke("onBattleStart", []);
    }
    public override void _OnBattleEnd()
    {
        ((IJavaScriptObject)this).Invoke("onBattleEnd", []);
    }
    public override void _OnDialogueStarting()
    {
        ((IJavaScriptObject)this).Invoke("onDialogueStarting", []);
    }
    public override void _OnDialogueEnding()
    {
        ((IJavaScriptObject)this).Invoke("onDialogueEnding", []);
    }

    public override void _HandleAttack(AttackStatus status)
	{
		((IJavaScriptObject)this).Invoke("onHandleAttack", [status]);
	}
	public override BaseBattleTurn _GetNextTurn()
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
				return IJavaScriptObject.New<JavaScriptBattleTurnProxy>(result);
			}
		}
		return new BaseBattleTurn();
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
