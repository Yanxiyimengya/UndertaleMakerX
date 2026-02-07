using Godot;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[GlobalClass]
public partial class JavaScriptBattleProjectileProxy : BaseBattleProjectile, IObjectPoolObject , IJavaScriptObject
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }

	public static IJavaScriptObject New(ObjectInstance objInstance)
	{
		JavaScriptBattleProjectileProxy projectile =
			UtmxBattleManager.GetBattleProjectileController().CreateProjectile<JavaScriptBattleProjectileProxy>();
		projectile.JsInstance = objInstance;
		return projectile;
	}
	public override void OnHitPlayer(BattlePlayerSoul playerSoul)
	{
		if (JsInstance.HasProperty("onHitPlayer"))
		{
			Invoke("onHitPlayer");
		}
		else
		{
			base.OnHitPlayer(playerSoul);
		}
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
		Invoke(EngineProperties.JAVASCRIPT_ACTIVE_CALLBACK);
	}
	private void OnDisabled()
	{
		Invoke(EngineProperties.JAVASCRIPT_DISABLED_CALLBACK);
	}
}
