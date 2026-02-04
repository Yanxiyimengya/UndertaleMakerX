using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System.Collections.Generic;

[GlobalClass]
public partial class JavaScriptTextTyperProxy : TextTyper, IObjectPoolObject , IJavaScriptObject
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }
	public static JavaScriptTextTyperProxy New(ObjectInstance objInstance)
	{
		JavaScriptTextTyperProxy typer = UtmxSceneManager.CreateTextTyper<JavaScriptTextTyperProxy>();
		typer.JsInstance = objInstance;
		return typer;
	}
	public override bool _ProcessCmd(string cmd, Dictionary<string, string> args)
	{
		bool result = false;
		if (JsInstance.HasProperty("processCmd"))
		{
            result = result || JavaScriptBridge.InvokeFunction(JsInstance, "processCmd", [cmd, args]).AsBoolean();
		}
        result = result || base._ProcessCmd(cmd, args);
		return result;
	}
}
