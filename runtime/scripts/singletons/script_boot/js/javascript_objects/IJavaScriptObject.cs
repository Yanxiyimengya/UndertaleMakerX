using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.IO;

public interface IJavaScriptObject
{
	public ObjectInstance JsInstance { get; set; }
	public string JsScriptPath { get; set; }

	// 这个方法直接从脚本实例化一个继承了实现IJavaScriptObject接口的Clr类型的JavaScript类
	// 也就是说只能从一个继承于实现了于IJavaScriptObject接口的JS类里实例化对象
	// 若脚本没有继承于正确的类型，就会返回null且打印错误
	public static T New<T>(string path) where T : class, IJavaScriptObject, new()
	{
		JavaScriptClass jsClass = JavaScriptBridge.FromFile(path);
		if (jsClass == null)
		{
			UtmxLogger.Error($"{TranslationServer.Translate("Try to load javascript module failed.")}: {path}");
			return null;
		}
		ObjectInstance jsInstance = jsClass.New();
		if (jsInstance == null)
		{
			UtmxLogger.Error($"{TranslationServer.Translate("Create javascript object instance failed.")}: {path}");
			return null;
		}

		T csharpObj = jsInstance.ToObject() as T;
		if (csharpObj == null)
		{
			UtmxLogger.Error($"{TranslationServer.Translate("The script type does not match the target type.")}: {jsInstance.GetType().ToString()}");
			return null;
		}
		csharpObj.JsInstance = jsInstance;
		csharpObj.JsScriptPath = path;
		return csharpObj;
	}

	// 这个方法从JS端包装JsValue类型,前提是JS端的对象继承于实现了IJavaScriptObject的Clr对象类型
	// 若脚本没有继承于正确的类型，就会返回null且打印错误
	public static T New<T>(JsValue value) where T : class, IJavaScriptObject, new()
	{
		T result = null;
		object obj = value.ToObject();
		if (obj != null && obj is IJavaScriptObject)
		{
			result = obj as T;
			result.JsInstance = value.AsObject();
		}
		if (result == null)
		{
			UtmxLogger.Error($"{TranslationServer.Translate("The script type does not match the target type.")}: {value.GetType().ToString()}");
		}
		return result;
	}


	public object Get(string propertyName)
	{
		JsValue jsValue = JsInstance.Get(propertyName);
		return JavaScriptBridge.ConvertToObject(jsValue);
	}

	public bool Has(string propertyName)
	{
		return JsInstance.HasProperty(propertyName);
	}

	public JsValue Invoke(string method, params object[] args)
	{
		if (string.IsNullOrEmpty(method)) return null;
		JsValue methodValue = JsInstance.Get(method);
		if (methodValue.Type == Jint.Runtime.Types.Object)
		{
			JsValue[] jsValues = new JsValue[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				jsValues[i] = JsValue.FromObject(JavaScriptBridge.MainEngine, args[i]);
			}
			return methodValue.Call(JsInstance, jsValues);
		}
		return null;
	}

	public static bool TryConvertToClr(in JsValue value, out IJavaScriptObject result)
	{
		result = value.ToObject() as IJavaScriptObject;
		if (result is null) return false;
		result.JsInstance = value as ObjectInstance;
		return true;
	}

	public static bool TryConvertToJsValue(in IJavaScriptObject value, out JsValue result)
	{
		result = JsValue.FromObject(JavaScriptBridge.MainEngine ,value);
		if (result == null) return false;
		return true;
	}
}
