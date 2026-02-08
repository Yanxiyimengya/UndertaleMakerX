using Jint.Native;
using Jint.Native.Object;
using System;

public partial class JavaScriptDrawableObjectProxy : DrawableObject, IObjectPoolObject, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }
    public static IJavaScriptObject New(ObjectInstance objInstance)
    {
        JavaScriptDrawableObjectProxy obj = UtmxSceneManager.CreateDrawableObject<JavaScriptDrawableObjectProxy>();
        obj.JsInstance = objInstance;
        if (((IJavaScriptObject)obj).Has(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK))
            ((IJavaScriptObject)obj).Invoke(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK, []);
        return obj;
    }

    public override void Awake()
    {
        base.Awake();
        CallDeferred(nameof(OnAwake));
    }
    public override void Disabled()
    {
        base.Disabled();
        CallDeferred(nameof(OnDisabled));
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
        Invoke(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, new object[] { delta });
    }
    public JsValue Invoke(string method, params object[] args)
    {
        if (JsInstance == null || string.IsNullOrEmpty(method))
            return null;
        if (JsInstance.HasProperty(method))
            return JavaScriptBridge.InvokeFunction(JsInstance, method, args);
        return null;
    }
    private void OnAwake()
    {
        SetProcess(JsInstance.HasProperty(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK));
        Invoke(EngineProperties.JAVASCRIPT_START_CALLBACK);
    }
    private void OnDisabled()
    {
        Invoke(EngineProperties.JAVASCRIPT_DISABLED_CALLBACK);
    }
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            if (((IJavaScriptObject)this).Has(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK))
                ((IJavaScriptObject)this).Invoke(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK, []);
        }
    }

}
