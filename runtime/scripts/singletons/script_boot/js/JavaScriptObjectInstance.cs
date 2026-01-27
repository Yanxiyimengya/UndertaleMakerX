using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public partial class JavaScriptObjectInstance : ScriptObjectInstance
{
	public JsObject JsInstance;

	public JavaScriptObjectInstance(JsObject ins)
	{
		JsInstance = ins;
	}

	public override object Get(string key)
	{
		return JsInstance.Get(key);
	}

	public override object Invoke(string method, object[] args)
	{
		if (string.IsNullOrEmpty(method)) return null;

		JsInstance.TryGetValue(method, out JsValue methodValue);
		if (!methodValue.IsUndefined() && !methodValue.IsNull())
		{
			JavaScriptBridge bridge = ScriptBoot.Instance.GetBridge<JavaScriptBridge>();
			JsValue[] jsValues = new JsValue[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				jsValues[i] = JsValue.FromObject(bridge.MainEngine, args[i]);
			}

			JsValue result = methodValue.Call(JsInstance, jsValues);
			return result.ToObject();
		}
		return null;
	}

	public override void Set(string key, object value)
	{
		JavaScriptBridge bridge = ScriptBoot.Instance.GetBridge<JavaScriptBridge>();
		if (bridge != null)
		{
			JsInstance.Set(key, JsValue.FromObject(bridge.MainEngine, value));
		}
	}
}
