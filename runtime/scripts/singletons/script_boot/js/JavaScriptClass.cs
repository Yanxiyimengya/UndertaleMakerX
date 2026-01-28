using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class JavaScriptClass : ScriptClass
{
    public ObjectInstance NamespaceObject;
    public JsValue JsConstructor;

    public override JavaScriptObjectInstance New()
    {
        try
        {
            JavaScriptBridge bridge = ScriptBoot.Instance.GetBridge<JavaScriptBridge>();
            if (bridge != null)
            {
                JsValue jsInstance = bridge.MainEngine.Construct(JsConstructor);
                if (jsInstance.Type == Jint.Runtime.Types.Object)
                {
                    return new JavaScriptObjectInstance((JsObject)jsInstance);
                }
            }
        }
        catch (JavaScriptException jsEx)
        {
            if (jsEx.Error.IsObject())
            {
                var errorObj = jsEx.Error.AsObject();
                UtmxLogger.Error($"{errorObj.Get("name")}: {errorObj.Get("message")}");
            }
            if (!string.IsNullOrEmpty(jsEx.Location.SourceFile))
            {
                UtmxLogger.Error($"SourceFile: {jsEx.Location.SourceFile} ({jsEx.Location.Start.Line}:{jsEx.Location.Start.Column})");
            }
            if (!string.IsNullOrEmpty(jsEx.JavaScriptStackTrace))
            {
                UtmxLogger.Error($"StackTrace: \n{jsEx.JavaScriptStackTrace}");
            }
            return null;
        }

        return null;
    }
}
