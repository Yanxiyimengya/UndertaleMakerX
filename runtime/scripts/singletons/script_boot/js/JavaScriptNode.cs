using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class JavaScriptNode : Node
{
	[Export(PropertyHint.FilePath, "*.js")]
	public string JavaScriptFile
	{
		get => _javaScriptFile;
		set
		{
			_javaScriptFile = value;
			_instance = null;
			if (!string.IsNullOrEmpty(_javaScriptFile))
			{
				JavaScriptClass javaScriptClass = JavaScriptBridge.FromFile(_javaScriptFile);
				if (javaScriptClass != null)
				{
					_instance = javaScriptClass.New();
					if (_instance.Has(EngineProperties.JAVASCRIPT_CREATE_CALLBACK))
						_instance.Invoke(EngineProperties.JAVASCRIPT_CREATE_CALLBACK, []);
					_javaScriptCanUpdate = _instance.Has(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK);
				}
			}
		}
	}

	private JavaScriptObjectInstance _instance;
	public string _javaScriptFile;
	public bool _javaScriptCanUpdate = false;

	public override Variant _Get(StringName property)
	{
		string propertyName = property.ToString();
		if (_instance.Has(propertyName))
		{
			object dotNetObject = _instance.Get(property);
			return JavaScriptBridge.ObjectConvertToVariant(dotNetObject);
		}
		return new Variant();
	}
	public override void _Ready()
	{
		if (_instance.Has(EngineProperties.JAVASCRIPT_ACTIVE_CALLBACK))
			_instance.Invoke(EngineProperties.JAVASCRIPT_ACTIVE_CALLBACK, []);
	}

	~JavaScriptNode()
	{
		if (_instance.Has(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK))
			_instance.Invoke(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK, []);
	}

	public override void _Process(double delta)
	{
		if (_javaScriptCanUpdate) 
			_instance.Invoke(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, [delta]);
	}
}
