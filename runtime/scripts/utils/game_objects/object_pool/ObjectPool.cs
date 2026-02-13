using Godot;
using System.Collections.Generic;
using static Godot.Node;

public partial class ObjectPool<T> : RefCounted where T : Godot.Node, IObjectPoolObject, new()
{
    private Queue<T> _pool = new();

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
        }
    }

    public T2 GetObject<T2>() where T2 : T, new()
    {
        T2 node = null;
        while (_pool.Count > 0)
        {
            var dequeued = _pool.Dequeue();
            if (dequeued is T2 target)
            {
                node = target;
                break;
            }
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
        node.ProcessMode = ProcessModeEnum.Inherit;
        node.Awake();
        if (node is CanvasItem ci) 
            ci.Show();
    }

    public void DisabledObject(T node)
    {
        node.Disabled();
        node.ProcessMode = ProcessModeEnum.Disabled;
        if (node is CanvasItem ci) ci.Hide();
        _pool.Enqueue(node);
    }
}