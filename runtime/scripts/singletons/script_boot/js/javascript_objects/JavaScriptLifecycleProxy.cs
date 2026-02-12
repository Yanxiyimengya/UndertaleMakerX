using Godot;
using Jint.Native;
using Jint.Native.Object;

[GlobalClass]
public partial class JavaScriptLifecycleProxy : Node
{
    public ObjectInstance JsInstance {
        get => _jsInstance;
        set
        {
            _jsInstance = value;
            SetProcess(JsInstance?.HasProperty(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK) ?? false);
            Invoke(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK);
            Invoke(EngineProperties.JAVASCRIPT_START_CALLBACK);
        }
    }
    private ObjectInstance _jsInstance = null;

    public override void _EnterTree()
    {
        CallDeferred(nameof(OnEnterTree));
    }

    public override void _Process(double delta)
    {
        Invoke(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, delta);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
            Invoke(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK);
    }

    public JsValue Invoke(string method, params object[] args)
    {
        if (JsInstance == null || string.IsNullOrEmpty(method))
            return null;
        if (JsInstance.HasProperty(method))
            return JavaScriptBridge.InvokeFunction(JsInstance, method, args);
        return null;
    }

    private void OnEnterTree()
    {
        Node parent = GetParent();
        if (parent is IJavaScriptLifecyucle lc)
        {
            JsInstance = lc.JsInstance;
        }
    }
}