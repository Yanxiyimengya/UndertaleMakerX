using Acornima.Ast;
using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;

public class JavaScriptBridge : ScriptBridge
{
	public static Jint.Engine MainEngine = 
		new Jint.Engine((options) =>
		{
			options.EnableModules(new JavaScriptModuleResolver());
			options.AllowClr();
			options.Interop.AllowGetType = true;
			options.Interop.AllowWrite = true;

		});
	static JavaScriptBridge()
	{
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

	public static JavaScriptClass FromFile(string path)
	{
		string filePath = JavaScriptModuleResolver.ResolvePath("", path);
		if (Godot.FileAccess.FileExists(filePath))
		{
			ObjectInstance jsNamespace = ImportModule(filePath);
			if (jsNamespace != null)
			{
				return _ConstructScriptObject(jsNamespace);
			}
		}
		return null;
	}

	public static JavaScriptClass ExecuteString(string code)
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

	private static JavaScriptClass _ConstructScriptObject(ObjectInstance jsNamespace)
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

	private static ObjectInstance ImportModule(string id)
	{
		try
		{
			ObjectInstance jsNamespace = MainEngine.Modules.Import(id);
			return jsNamespace;
		}
		catch (Exception ex) when (!(ex is JavaScriptException))
		{
			UtmxLogger.Error($"{TranslationServer.Translate("Try to load javascript module failed")}: {id}");
			UtmxLogger.Error($"{TranslationServer.Translate("OuterException Typ")}: {ex.GetType().FullName}");
			UtmxLogger.Error($"{TranslationServer.Translate("OuterException Message")}: {ex.Message}");
			UtmxLogger.Error($"{TranslationServer.Translate(".NET StackTrace")}: \n{ex.StackTrace}");

			if (ex.InnerException != null)
			{
				UtmxLogger.Error($"{TranslationServer.Translate("InnerException Type")}: {ex.InnerException.GetType().FullName}");
				UtmxLogger.Error($"{TranslationServer.Translate("InnerException Message")}: {ex.InnerException.Message}");
				UtmxLogger.Error($"{TranslationServer.Translate("InnerException StackTrace")}:\n{ex.InnerException.StackTrace}");
			}
			return null;
		}
	}

	public static object ConvertToObject(JsValue jsValue)
	{
		if (jsValue is Function)
		{
			return ((Function)jsValue).ToObject();
		}
		if (jsValue is JsObject)
		{
			Dictionary<string, object> dict = new();
			ObjectInstance jsObject = jsValue.AsObject();
			foreach (var entry in jsObject.GetOwnProperties())
			{
				dict[entry.Key.ToString()] = entry.Value.Value.ToObject();
			}
			return dict;
		}
		return jsValue.ToObject();
	}

	public static Variant ObjectConvertToVariant(object value)
	{
		if (value == null) return new Variant();
		if (value is bool b) return Variant.From(b);
		if (value is int i) return Variant.From(i);
		if (value is double d) return Variant.From(d);
		if (value is float f) return Variant.From(f);
		if (value is string s) return Variant.From(s);
		if (value is long l) return Variant.From(l);
		if (value is object[] arr)
		{
			Godot.Collections.Array<Variant> result = new();
			foreach (object obj in arr)
			{
				result.Add(ObjectConvertToVariant(obj));
			}
			return result;
		}
		if (value is Dictionary<string, object> dict)
		{
			Godot.Collections.Dictionary result = new();
			foreach (KeyValuePair<string, object> pair in dict)
			{
				result.Add(pair.Key, ObjectConvertToVariant(pair.Value));
			}
			return result;
		}
		return value.ToString();
	}

}
