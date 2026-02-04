using Godot;
using System.Collections.Generic;
using static Godot.Node;

public partial class ObjectPool<T> : RefCounted where T : Godot.Node, IObjectPoolObject, new()
{
    private Queue<T> _pool = new();

    ~ObjectPool()
    {
        foreach (T node in _pool)
        {
            if (node != null && IsInstanceIdValid(node.GetInstanceId()))
            {
                node.QueueFree();
            }
        }
    }

    public T2 GetObject<T2>() where T2 : T, new()
    {
        T2 node;
        if (_pool.Count > 0)
        {
            node = (T2)_pool.Dequeue();

            if (node is CanvasItem canvasItem)
            {
                canvasItem.Visible = true;
            }
            node.ProcessMode = ProcessModeEnum.Inherit;
            node.SetProcess(true);
            node.SetPhysicsProcess(true);
        }
        else
        {
            node = new T2();
        }
        AppendNode(node);
        return node;
    }

    public void AppendNode(T node)
    {
        node.ProcessMode = ProcessModeEnum.Inherit;
        node.SetProcess(true);
        node.SetPhysicsProcess(true);
        node.Awake();
        if (node is CanvasItem canvasItem) canvasItem.Show();
    }

    public void DisabledObject(T node)
    {
        node.Disabled();
        node.ProcessMode = ProcessModeEnum.Disabled;
        node.SetProcess(false);
        node.SetPhysicsProcess(false);
        if (node is CanvasItem canvasItem) canvasItem.Hide();
        _pool.Enqueue(node);
    }
}