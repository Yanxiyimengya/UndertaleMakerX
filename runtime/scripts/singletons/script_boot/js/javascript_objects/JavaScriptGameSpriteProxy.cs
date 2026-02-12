using Godot;
using Jint.Native;
using Jint.Native.Object;

[GlobalClass]
public partial class JavaScriptGameSpriteProxy : GameSprite2D, IJavaScriptLifecyucle
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }
    public JavaScriptLifecycleProxy LifecycleProxy { get; set; } = new();
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
