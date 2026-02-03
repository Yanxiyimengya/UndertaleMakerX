using Godot;
using Jint.Native.Object;

[GlobalClass]
public partial class JavaScriptSpriteProxy : GameSprite2D, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }
    public static JavaScriptSpriteProxy New(ObjectInstance objInstance, bool mask = false)
    {
        JavaScriptSpriteProxy sprite = new();
        sprite.JsInstance = objInstance;
        return sprite;
    }

}