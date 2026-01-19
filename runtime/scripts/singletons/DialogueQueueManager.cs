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
		public Dialogue(string msg) { 
			Message = msg;
		}
	}
	private Queue<Dialogue> _dialogueQueue = new Queue<Dialogue>();

	public void AppendDialogue(string msg)
	{
		_dialogueQueue.Enqueue(new Dialogue(msg));
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
