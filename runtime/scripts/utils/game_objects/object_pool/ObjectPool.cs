using Godot;
using System.Collections.Generic;
using static Godot.Node;

public partial class ObjectPool<T> : RefCounted where T : Godot.Node, IObjectPoolObject, new()
{
    private Queue<T> _pool = new();
    private HashSet<T> _pooledNodes = new();

    private static bool IsReusableNode(T node)
    {
        return node != null && IsInstanceValid(node) && !node.IsQueuedForDeletion();
    }

    private static bool IsPooledStateNode(T node)
    {
        return IsReusableNode(node) && node.ProcessMode == ProcessModeEnum.Disabled;
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            foreach (T node in _pool)
            {
                if (node != null && IsInstanceValid(node))
                {
                    node.QueueFree();
                }
            }
            _pool.Clear();
            _pooledNodes.Clear();
        }
    }

    public T2 GetObject<T2>() where T2 : T, new()
    {
        T2 node = null;
        int scanCount = _pool.Count;
        for (int i = 0; i < scanCount; i++)
        {
            var dequeued = _pool.Dequeue();
            _pooledNodes.Remove(dequeued);
            if (!IsPooledStateNode(dequeued))
            {
                continue;
            }

            if (dequeued is T2 target)
            {
                node = target;
                break;
            }

            // Keep non-target pooled objects in the queue.
            _pool.Enqueue(dequeued);
            _pooledNodes.Add(dequeued);
        }
        if (node == null)
        {
            node = new T2();
        }
        AppendNode(node);
        return node;
    }
    public void AppendNode(T node)
    {
        if (!IsReusableNode(node)) return;
        node.ProcessMode = ProcessModeEnum.Inherit;
        node.Awake();
        if (node is CanvasItem ci)
            ci.Show();
    }

    public void DisabledObject(T node)
    {
        if (!IsReusableNode(node)) return;

        if (_pooledNodes.Contains(node))
            return;

        // If node is already disabled, treat it as pooled and skip re-enqueue.
        if (node.ProcessMode == ProcessModeEnum.Disabled)
        {
            _pooledNodes.Add(node);
            return;
        }

        node.Disabled();
        node.ProcessMode = ProcessModeEnum.Disabled;
        if (node is CanvasItem ci) ci.Hide();
        _pool.Enqueue(node);
        _pooledNodes.Add(node);
    }
}
