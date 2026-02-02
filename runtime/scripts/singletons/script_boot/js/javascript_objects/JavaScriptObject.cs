using Godot;
using Jint.Native;
using Jint.Native.Object;

public class JavaScriptObject : IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }
}
