using Godot;
using Jint.Native;
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

    public override void Set(string key, object value)
    {
        JavaScriptBridge bridge = ScriptBoot.Instance.GetBridge<JavaScriptBridge>();
        if (bridge != null)
        {
            JsInstance.Set(key, JsValue.FromObject(bridge.MainEngine, value));
        }
    }
}

