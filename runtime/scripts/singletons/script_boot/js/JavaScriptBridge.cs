using Acornima.Ast;
using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Map;
using Jint.Native.Object;
using System;
using System.Xml.Linq;

public class JavaScriptBridge : ScriptBridge
{
	public Jint.Engine MainEngine;
	public string ModulePath = $"{EngineProperties.DATAPACK_RESOURCE_PATH}";

	public JavaScriptBridge()
	{
		MainEngine = new Jint.Engine((options) =>
		{
			options.EnableModules(new JavaScriptModuleResolver());
		});

		MainEngine.Modules.Add("UTMX", builder => builder
			.ExportType<JavaScriptGlobalInterface>("Core")
		);
	}

	public JavaScriptClass FromFile(string uri)
	{
		ObjectInstance jsNamespace = MainEngine.Modules.Import(uri);
		return _ConstructScriptObject(jsNamespace);
	}

	public override JavaScriptClass ExecuteString(string code)
	{
		string moduleId = code.Sha256Text();
		MainEngine.Modules.Add(moduleId, code);
		ObjectInstance jsNamespace = MainEngine.Modules.Import(moduleId);
		return _ConstructScriptObject(jsNamespace);
	}

	private JavaScriptClass _ConstructScriptObject(ObjectInstance jsNamespace)
	{
		JsValue defaultValue = jsNamespace.Get("default");
		if (defaultValue.Type == Jint.Runtime.Types.Object)
		{
			JavaScriptClass scriptObj = new JavaScriptClass();
			scriptObj.NamespaceObject = jsNamespace;
			scriptObj.JsConstructor = defaultValue;
			return scriptObj;
		}
		else
		{
		}
		return null;
	}

	public override object GetValue(string value)
	{
		return MainEngine.Evaluate(value).ToObject();
	}

	public override void SetValue(string valueName, object value)
	{
		MainEngine.SetValue(valueName, value);
	}
}
