using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class DialogueQueueManager
{
	private static readonly Lazy<DialogueQueueManager> _instance =
		new Lazy<DialogueQueueManager>(() => new DialogueQueueManager());
	private DialogueQueueManager() { }
	public static DialogueQueueManager Instance => _instance.Value;
	public partial class Dialogue : RefCounted
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
		if (!_enemysDialogueQueues.ContainsKey(enemyIndex))
			_enemysDialogueQueues.Add(enemyIndex, new Queue<Dialogue>());
		_enemysDialogueQueues[enemyIndex].Enqueue(dialogue);
	}

	public int BattleEnemyDialogueCount()
	{
		return _enemysDialogueQueues.Count;
	}

	public Dictionary<int, Dialogue> GetBattleEnemyDialogues()
	{
		Dictionary<int, Dialogue> result = new Dictionary<int, Dialogue>();
		List<int> keysToRemove = new List<int>();
		foreach (KeyValuePair<int, Queue<Dialogue>> keyValuePair in _enemysDialogueQueues)
		{
			int enemyKey = keyValuePair.Key;
			Queue<Dialogue> dialogueQueue = keyValuePair.Value;
			if (dialogueQueue != null && dialogueQueue.Count > 0)
			{
				result[enemyKey] = dialogueQueue.Dequeue();
				if (dialogueQueue.Count == 0)
				{
					keysToRemove.Add(enemyKey);
				}
			}
			else
			{
				keysToRemove.Add(enemyKey);
			}
		}
		foreach (int key in keysToRemove)
		{
			if (_enemysDialogueQueues.ContainsKey(key))
			{
				_enemysDialogueQueues.Remove(key);
			}
		}
		return result;
	}
	public Dictionary<int, string> GetBattleEnemyDialoguesAsText()
	{
		Dictionary<int, Dialogue> dialogues = GetBattleEnemyDialogues();
		Dictionary<int, string> result = new();
		foreach (KeyValuePair<int, Dialogue> keyValuePair in dialogues)
		{
			result[keyValuePair.Key] = keyValuePair.Value.Message;
		}
		return result;
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
