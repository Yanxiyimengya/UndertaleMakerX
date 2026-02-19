using Godot;
using Jint.Native;
using Jint.Native.Object;

[GlobalClass]
public partial class JavaScriptGameSpriteProxy : GameSprite2D, IJavaScriptLifecyucle
{
	public ObjectInstance JsInstance
	{
		get => _jsInstance;
		set
		{
			_jsInstance = value;
			if (LifecycleProxy != null)
				LifecycleProxy.JsInstance = value;
		}
	}
	public string JsScriptPath { get; set; }
    public JavaScriptLifecycleProxy LifecycleProxy { get; set; } = new();
	private ObjectInstance _jsInstance = null;
    public override void _Ready()
    {
        base._Ready();
        AddChild(LifecycleProxy);
    }
    public static JavaScriptGameSpriteProxy New(ObjectInstance objInstance)
	{
		JavaScriptGameSpriteProxy sprite = UtmxSceneManager.CreateSprite<JavaScriptGameSpriteProxy>();
		sprite.JsInstance = objInstance;
        if (((IJavaScriptObject)sprite).Has(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK))
			((IJavaScriptObject)sprite).Invoke(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK, []);
		return sprite;
	}
}
