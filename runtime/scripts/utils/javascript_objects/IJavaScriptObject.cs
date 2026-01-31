using Godot;

public interface IJavaScriptObject
{
	JavaScriptObjectInstance JsInstance { get; set; }
	public static T New<T>(string path) where T : class, IJavaScriptObject, new()
	{
		JavaScriptClass jsClass = JavaScriptBridge.FromFile(path);
		if (jsClass == null)
		{
			UtmxLogger.Error($"{TranslationServer.Translate("Try to load javascript module failed.")}: {path}");
			return null;
		}

		JavaScriptObjectInstance jsInstance = jsClass.New();
		if (jsInstance == null)
		{
			UtmxLogger.Error($"{TranslationServer.Translate("Create javascript object instance failed.")}: {path}");
			return null;
		}

		T csharpObj = jsInstance.ToObject() as T;
		if (csharpObj == null)
		{
			UtmxLogger.Error($"{TranslationServer.Translate("The script type does not match the target type.")}: {path}");
			return null;
		}

		csharpObj.JsInstance = jsInstance;
		return csharpObj;
	}

	public Variant _Get(StringName property)
	{
		string propertyName = property.ToString();
		if (JsInstance.Has(propertyName))
		{
			object dotNetObject = JsInstance.Get(property);
			return JavaScriptBridge.ObjectConvertToVariant(dotNetObject);
		}
		return new Variant();
	}
}
