using Godot;
using Jint.Native.Object;

public partial class JavaScriptSprite2DProxy : Sprite2D, IJavaScriptObject
{
    ObjectInstance IJavaScriptObject.JsInstance { get; set; }
    string IJavaScriptObject.JsScriptPath { get; set; }


}
