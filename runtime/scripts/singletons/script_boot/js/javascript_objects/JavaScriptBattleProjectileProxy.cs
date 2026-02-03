using Godot;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class JavaScriptBattleProjectileProxy : BaseBattleProjectile, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }

    public static JavaScriptBattleProjectileProxy New(ObjectInstance objInstance, bool mask = false)
    {
        JavaScriptBattleProjectileProxy projectile = 
            UtmxBattleManager.GetBattleProjectileController().CreateProjectile<JavaScriptBattleProjectileProxy>(mask);
        projectile.JsInstance = objInstance;
        return projectile;
    }

    public override void _Process(double delta)
    {
        ((IJavaScriptObject)this).Invoke(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, [delta]);
    }
}