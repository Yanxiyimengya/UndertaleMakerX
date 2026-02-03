using Godot;
using Jint.Native.Object;

[GlobalClass]
public partial class JavaScriptSpriteProxy : BaseBattleProjectile
{
    public JavaScriptSpriteProxy(bool mask = false)
    {
        if (UtmxBattleManager.IsInBattle())
        {
            SceneTree sceneTree = UtmxSceneManager.Instance.GetTree();
            if (sceneTree != null)
            {

            }
        }
        else
        {
            QueueFree();
        }
    }

    public void SetTextures(object value)
    {
        SetTextures((Texture[])value);
    }
}