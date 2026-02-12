using Godot;
using Jint.Native;
using Jint.Native.Object;
using System;

public partial class JavaScriptDrawableObjectProxy : DrawableObject, IObjectPoolObject, IJavaScriptLifecyucle
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }
    public JavaScriptLifecycleProxy LifecycleProxy { get; set; } = new();
    public override void _Ready()
    {
        base._Ready();
        AddChild(LifecycleProxy);
    }

    public static JavaScriptDrawableObjectProxy New(ObjectInstance objInstance)
	{
		JavaScriptDrawableObjectProxy obj = UtmxSceneManager.CreateDrawableObject<JavaScriptDrawableObjectProxy>();
		obj.JsInstance = objInstance;
        if (((IJavaScriptObject)obj).Has(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK))
			((IJavaScriptObject)obj).Invoke(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK, []);
		return obj;
	}
}
