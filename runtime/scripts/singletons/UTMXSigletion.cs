using Godot;

public partial class UTMXSingleton<T> : Node
    where T : Node, new()
{
    public static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance is not null) 
                return _instance;
            _instance = new T();
            return _instance;
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();
		_instance = this as T;
    }
}