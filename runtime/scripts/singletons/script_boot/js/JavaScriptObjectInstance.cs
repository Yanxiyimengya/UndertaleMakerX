using Jint.Native;
using Godot;
using Jint;

public partial class JavaScriptObjectInstance : ScriptObjectInstance
{
	public JsObject JsInstance;

	public JavaScriptObjectInstance(JsObject ins)
	{
		JsInstance = ins;
		JsInstance.Set("this", JsValue.FromObject(JavaScriptBridge.MainEngine, this));
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

	public override object Invoke(string method, object[] args)
	{
		if (string.IsNullOrEmpty(method)) return null;
		JsInstance.TryGetValue(method, out JsValue methodValue);
		if (!methodValue.IsUndefined() && !methodValue.IsNull())
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

	public override void Set(string key, object value)
	{
		JsInstance.Set(key, JsValue.FromObject(JavaScriptBridge.MainEngine, value));
	}
}
