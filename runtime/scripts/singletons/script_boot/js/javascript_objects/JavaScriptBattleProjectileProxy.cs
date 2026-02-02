using Godot;
using Jint.Native.Object;

[GlobalClass]
public partial class JavaScriptBattleProjectileProxy : BaseBattleProjectile, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }

}