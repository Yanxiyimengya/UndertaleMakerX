using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System.Collections.Generic;

[GlobalClass]
public partial class JavaScriptTextTyperProxy : TextTyper, IObjectPoolObject, IJavaScriptObject
{
    public ObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }
    public static IJavaScriptObject New(ObjectInstance objInstance)
    {
        JavaScriptTextTyperProxy typer = UtmxSceneManager.CreateTextTyper<JavaScriptTextTyperProxy>();
        typer.JsInstance = objInstance;
        return typer;
    }
    public override bool _ProcessCmd(string cmd, Dictionary<string, string> args)
    {
        if (JsInstance != null && JsInstance.HasProperty("processCmd"))
        {
            JsValue result = JavaScriptBridge.InvokeFunction(JsInstance, "processCmd", [cmd, args]);
            if (!result.IsUndefined() && !result.IsNull())
                if (result.AsBoolean() == true) return true;
        }
        return base._ProcessCmd(cmd, args);
    }
}
