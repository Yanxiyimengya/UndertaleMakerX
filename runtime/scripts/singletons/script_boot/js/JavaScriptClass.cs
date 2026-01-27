using Jint.Native;
using Jint.Native.Object;
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
		JavaScriptBridge bridge = ScriptBoot.Instance.GetBridge<JavaScriptBridge>();
		if (bridge != null)
		{
			JsValue jsInstance = bridge.MainEngine.Construct(JsConstructor);
			if (jsInstance.Type == Jint.Runtime.Types.Object)
			{
				return new JavaScriptObjectInstance((JsObject)jsInstance);
			}
		}
		return null;
	}
}
