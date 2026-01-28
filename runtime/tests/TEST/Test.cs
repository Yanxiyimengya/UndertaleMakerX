using Godot;

public partial class Test : Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        JavaScriptClass cls = ScriptBoot.Instance.GetBridge<JavaScriptBridge>().ExecuteString("export default class MyClass { a = 10;}");
        JavaScriptObjectInstance JsObject = cls.New();
        GD.Print(JsObject.Get("a"));
        JsObject.Set("猪程度", 100);
        GD.Print(JsObject.Get("猪程度"));



    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
