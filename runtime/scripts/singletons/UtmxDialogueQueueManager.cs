using Godot;
using System;
using System.Collections.Generic;

public partial class UtmxDialogueData : RefCounted
{
	public string Message = "";
	public Dictionary<string, Variant> MetaData;
	public Func<string, Dictionary<string, string>, bool> ProcessCmdCallback = null;
	public UtmxDialogueData(string msg)
	{
		Message = msg;
		MetaData = new Dictionary<string, Variant>();
	}

	public bool TryGetMetaData(string key, out Variant value)
	{
		return MetaData.TryGetValue(key, out value);
	}
}
public partial class UtmxDialogueQueueManager
{
	private static Queue<UtmxDialogueData> _dialogueQueue = new Queue<UtmxDialogueData>();
	private static Dictionary<int, Queue<UtmxDialogueData>> _enemysDialogueQueues = new Dictionary<int, Queue<UtmxDialogueData>>();

	public static void AppendBattleEnemyDialogue(
		int enemyIndex,
		string dialogueMessage,
		Vector2 pos,
		Vector2 size,
		Func<string, Dictionary<string, string>, bool> processCmdCallback = null
	)
	{
		UtmxDialogueData dialogue = new UtmxDialogueData(dialogueMessage);
		dialogue.ProcessCmdCallback = processCmdCallback;
		dialogue.MetaData = new Dictionary<string, Variant> {
			{"Poisition" , pos },
            {"Size" , size }
		};
		if (!_enemysDialogueQueues.ContainsKey(enemyIndex))
			_enemysDialogueQueues.Add(enemyIndex, new Queue<UtmxDialogueData>());
		_enemysDialogueQueues[enemyIndex].Enqueue(dialogue);
	}

	public static int BattleEnemyDialogueCount()
	{
		return _enemysDialogueQueues.Count;
	}

	public static Dictionary<int, UtmxDialogueData> GetBattleEnemyDialogues()
	{
		Dictionary<int, UtmxDialogueData> result = new Dictionary<int, UtmxDialogueData>();
		List<int> keysToRemove = new List<int>();
		foreach (KeyValuePair<int, Queue<UtmxDialogueData>> keyValuePair in _enemysDialogueQueues)
		{
			int enemyKey = keyValuePair.Key;
			Queue<UtmxDialogueData> dialogueQueue = keyValuePair.Value;
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
	public static Dictionary<int, string> GetBattleEnemyDialoguesAsText()
	{
		Dictionary<int, UtmxDialogueData> dialogues = GetBattleEnemyDialogues();
		Dictionary<int, string> result = new();
		foreach (KeyValuePair<int, UtmxDialogueData> keyValuePair in dialogues)
		{
			result[keyValuePair.Key] = keyValuePair.Value.Message;
		}
		return result;
	}

	public static void AppendDialogue(string msg, Dictionary<string, Variant> mataData = null)
	{
		UtmxDialogueData dialogue = new UtmxDialogueData(msg);
		if (mataData != null)
			dialogue.MetaData = mataData;
		_dialogueQueue.Enqueue(dialogue);
	}
	public static void AppendDialogue(string[] msgs, Dictionary<string, Variant> mataData = null)
	{
		foreach (string msg in msgs)
		{
			UtmxDialogueData dialogue = new UtmxDialogueData(msg);
			if (mataData != null)
				dialogue.MetaData = mataData;
			_dialogueQueue.Enqueue(dialogue);
		}
	}
	public static void ClearDialogue()
	{
		_dialogueQueue.Clear();
	}

	public static int DialogueCount()
	{
		return _dialogueQueue.Count;
	}

	public static string GetNextDialogueAsText()
	{
		return _dialogueQueue.Dequeue().Message;
	}
}
