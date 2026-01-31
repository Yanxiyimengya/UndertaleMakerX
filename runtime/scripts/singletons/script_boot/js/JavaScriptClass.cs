using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.HttpRequest;

public partial class JavaScriptClass : ScriptClass
{
    public ObjectInstance NamespaceObject;
    public JsValue JsConstructor;

    public override JavaScriptObjectInstance New(params object[] arguments)
    {
        try
        {
            JsValue[] jsArgs = arguments.Select(arg => JsValue.FromObject(JavaScriptBridge.MainEngine, arg)).ToArray();
            JsValue jsInstance = JavaScriptBridge.MainEngine.Construct(JsConstructor, jsArgs);
            if (jsInstance.Type == Jint.Runtime.Types.Object)
            {
                ObjectInstance jsObject = jsInstance.AsObject();
                return new JavaScriptObjectInstance(jsObject);
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
                UtmxLogger.Error($"{TranslationServer.Translate("SourceFile")}: {jsEx.Location.SourceFile} ({jsEx.Location.Start.Line}:{jsEx.Location.Start.Column})");
            }
            if (!string.IsNullOrEmpty(jsEx.JavaScriptStackTrace))
            {
                UtmxLogger.Error($"{TranslationServer.Translate("StackTrace")}: \n{jsEx.JavaScriptStackTrace}");
            }
            return null;
        }

        return null;
    }
}
