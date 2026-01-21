using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class DialogueQueueManager : Node
{
	private static readonly Lazy<DialogueQueueManager> _instance =
		new Lazy<DialogueQueueManager>(() => new DialogueQueueManager());
	private DialogueQueueManager() { }
	public static DialogueQueueManager Instance => _instance.Value;
	public struct Dialogue
	{
		public string Message = "";
		public Dictionary<string, Variant> MetaData;
		public Dialogue(string msg) { 
			Message = msg;
            MetaData = new Dictionary<string, Variant>();
        }

		public bool TryGetMetaData(string key, out Variant value)
		{
			return MetaData.TryGetValue(key, out value);
        }
	}
	private Queue<Dialogue> _dialogueQueue = new Queue<Dialogue>();
    private Dictionary<int, Queue<Dialogue>> _enemysDialogueQueues = new Dictionary<int, Queue<Dialogue>>();

    public void AppendDialogue(string msg, Dictionary<string, Variant> mataData = null)
	{
		Dialogue dialogue = new Dialogue(msg);
		if (mataData != null)
            dialogue.MetaData = mataData;
        _dialogueQueue.Enqueue(dialogue);
	}

	public void AppendBattleEnemyDialogue(int enemyIndex ,string dialogueMessage, Vector2 pos, bool hideSpike = false, int dir = 2)
    {
        Dialogue dialogue = new Dialogue(dialogueMessage);
        dialogue.MetaData = new Dictionary<string, Variant> {
            {"Poisition" , pos },
            {"Dir" , dir },
            {"HideSpike" , hideSpike },
        };
        _enemysDialogueQueues[enemyIndex].Enqueue(dialogue);
    }

	public int DialogueCount()
	{
		return _dialogueQueue.Count;
	}

	public Dialogue GetNextDialogue()
	{
		return _dialogueQueue.Dequeue();
	}

	public string GetNextDialogueAsText()
	{
		return _dialogueQueue.Dequeue().Message;
	}
}
