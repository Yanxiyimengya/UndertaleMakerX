using Godot;
using Jint.Native;
using Jint.Native.Object;

[GlobalClass]
public partial class JavaScriptGameSpriteProxy : GameSprite2D, IJavaScriptObject
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }
	public static JavaScriptGameSpriteProxy New(ObjectInstance objInstance)
	{
		JavaScriptGameSpriteProxy sprite = UtmxSceneManager.CreateSprite<JavaScriptGameSpriteProxy>();
		sprite.JsInstance = objInstance;
		if (((IJavaScriptObject)sprite).Has(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK))
			((IJavaScriptObject)sprite).Invoke(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK, []);
		return sprite;
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
}
