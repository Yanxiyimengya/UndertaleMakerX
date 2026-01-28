using Acornima.Ast;
using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Map;
using Jint.Native.Object;
using Jint.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;

public class JavaScriptBridge : ScriptBridge
{
	public Jint.Engine MainEngine;

	public JavaScriptBridge()
	{
		MainEngine = new Jint.Engine((options) =>
		{
			options.EnableModules(new JavaScriptModuleResolver());
		});

		MainEngine.Modules.Add(JavaScriptCoreInterface.ModuleName, (builder) =>
		{
			Dictionary<string, Type> exportTypes = JavaScriptCoreInterface._GetCoreExportTypes();
			var method = typeof(Jint.Runtime.Modules.ModuleBuilder).GetMethod("ExportType",
				BindingFlags.Public | BindingFlags.Instance,
				null,
				new Type[] { typeof(string) },
				null);
			if (method != null)
			{
				foreach (KeyValuePair<string, Type> kvp in exportTypes)
				{
					var genericMethod = method.MakeGenericMethod(kvp.Value);
					genericMethod.Invoke(builder, new object[] { kvp.Key });
				}
			}
		});

		Dictionary<string, string> exportModuleScripts = JavaScriptCoreInterface._GetInterfaceExportScripts();
		foreach (KeyValuePair<string, string> kvp in exportModuleScripts)
		{
			if (Godot.FileAccess.FileExists(kvp.Value))
			{
				Godot.FileAccess access = Godot.FileAccess.Open(kvp.Value, Godot.FileAccess.ModeFlags.Read);
				if (access != null)
				{
					string fileContent = access.GetBuffer((long)access.GetLength()).GetStringFromUtf8();
					MainEngine.Modules.Add(kvp.Key, fileContent);
				}
			}
		}
	}

	public JavaScriptClass FromFile(string path)
	{
		if (Godot.FileAccess.FileExists(path))
		{
			ObjectInstance jsNamespace = ImportModule(path);
			if (jsNamespace != null)
			{
				return _ConstructScriptObject(jsNamespace);
			}
		}
		return null;
	}

	public override JavaScriptClass ExecuteString(string code)
	{
		string moduleId = code.Sha256Text();
		MainEngine.Modules.Add(moduleId, code);
		ObjectInstance jsNamespace = ImportModule(moduleId);
		if (jsNamespace != null)
		{
			return _ConstructScriptObject(jsNamespace);
		}
		return null;
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
		return null;
	}

	private ObjectInstance ImportModule(string id)
	{
		try
		{
			ObjectInstance jsNamespace = MainEngine.Modules.Import(id);
			return jsNamespace;
		}
		catch (Exception ex) when (!(ex is JavaScriptException))
		{
			UtmxLogger.Error($"内部异常: {ex.GetType().FullName}");
			UtmxLogger.Error($"错误消息: {ex.Message}");
			UtmxLogger.Error($".NET堆栈:\n{ex.StackTrace}");
			if (ex.InnerException != null)
			{
				UtmxLogger.Error($"内部异常: {ex.InnerException.GetType().FullName}");
				UtmxLogger.Error($"内部异常消息: {ex.InnerException.Message}");
				UtmxLogger.Error($"内部异常堆栈:\n{ex.InnerException.StackTrace}");
			}
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
