using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class JavaScriptNode : Node, IJavaScriptObject
{
	public JavaScriptObjectInstance JsInstance { get; set; }
    public string JsScriptPath { get; set; }

    [Export(PropertyHint.FilePath, "*.js")]
	public string JavaScriptFile
	{
		get => _javaScriptFile;
		set
		{
			_javaScriptFile = value;
			JsInstance = null;
			if (!string.IsNullOrEmpty(_javaScriptFile))
			{
				JavaScriptClass javaScriptClass = JavaScriptBridge.FromFile(_javaScriptFile);
				if (javaScriptClass != null)
				{
					JsInstance = javaScriptClass.New();
					if (JsInstance.Has(EngineProperties.JAVASCRIPT_CREATE_CALLBACK))
						JsInstance.Invoke(EngineProperties.JAVASCRIPT_CREATE_CALLBACK, []);
					_javaScriptCanUpdate = JsInstance.Has(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK);
				}
			}
		}
	}
	public string _javaScriptFile;
	public bool _javaScriptCanUpdate = false;

    public JavaScriptNode New(string path)
    {
		JavaScriptNode result = new JavaScriptNode();
		JsScriptPath = path;
        result.JavaScriptFile = path;
        return result;
    }
	
	public override void _Ready()
	{
		if (JsInstance.Has(EngineProperties.JAVASCRIPT_ACTIVE_CALLBACK))
			JsInstance.Invoke(EngineProperties.JAVASCRIPT_ACTIVE_CALLBACK, []);
	}

	~JavaScriptNode()
	{
		if (JsInstance.Has(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK))
			JsInstance.Invoke(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK, []);
	}

	public override void _Process(double delta)
	{
		if (_javaScriptCanUpdate) 
			JsInstance.Invoke(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, [delta]);
	}
}
