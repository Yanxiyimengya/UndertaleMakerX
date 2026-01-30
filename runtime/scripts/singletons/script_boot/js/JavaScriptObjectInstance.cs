using Jint.Native;
using Godot;
using Jint;
using Jint.Native.Object;
using System.Data.SqlTypes;

public partial class JavaScriptObjectInstance : ScriptObjectInstance
{
	public ObjectInstance JsInstance;

	public JavaScriptObjectInstance(ObjectInstance ins)
	{
		JsInstance = ins;
	}

	public override object Get(string propertyName)
	{
		JsValue jsValue = JsInstance.Get(propertyName);
		return JavaScriptBridge.ConvertToObject(jsValue);
	}

	public override bool Has(string propertyName)
	{
		return JsInstance.HasProperty(propertyName);
	}

	public override object Invoke(string method, params object[] args)
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

			JsValue result = methodValue.Call(JsInstance, jsValues);
			return result.ToObject();
		}
		return null;
	}

	public object ToObject()
	{
		return JsInstance.ToObject();
	}

	public override void Set(string key, object value)
	{
		JsInstance.Set(key, JsValue.FromObject(JavaScriptBridge.MainEngine, value));
	}
}
