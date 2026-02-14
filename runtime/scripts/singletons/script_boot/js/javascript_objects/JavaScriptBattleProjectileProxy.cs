using Godot;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[GlobalClass]
public partial class JavaScriptBattleProjectileProxy : BaseBattleProjectile, IObjectPoolObject , IJavaScriptLifecyucle
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }
    public JavaScriptLifecycleProxy LifecycleProxy { get; set; } = new();
    public override void _Ready()
    {
        base._Ready();
        AddChild(LifecycleProxy);
    }

    public static IJavaScriptObject New(ObjectInstance objInstance)
	{
		JavaScriptBattleProjectileProxy projectile =
			UtmxBattleManager.GetBattleProjectileController().CreateProjectile<JavaScriptBattleProjectileProxy>();
		projectile.JsInstance = objInstance;
        if (((IJavaScriptObject)projectile).Has(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK))
            ((IJavaScriptObject)projectile).Invoke(EngineProperties.JAVASCRIPT_ON_LOAD_CALLBACK, []);
        return projectile;
	}
	public override void OnHitPlayer(BattlePlayerSoul playerSoul)
	{
		if (JsInstance.HasProperty("onHit"))
        {
            ((IJavaScriptObject)this).Invoke("onHit");
        }
		else
		{
			base.OnHitPlayer(playerSoul);
		}
	}

    public override void OnHitProjectile(BaseBattleProjectile projectile)
    {
        if (JsInstance.HasProperty("onHitProjectile"))
        {
            ((IJavaScriptObject)this).Invoke("onHitProjectile", [projectile]);
        }
        //else base.OnHitProjectile(projectile);
    }

}
