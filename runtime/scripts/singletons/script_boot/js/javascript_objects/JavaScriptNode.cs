using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class JavaScriptNode : Node, IJavaScriptObject
{
	public ObjectInstance JsInstance { get; set; }
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
					if (((IJavaScriptObject)this).Has(EngineProperties.JAVASCRIPT_CREATE_CALLBACK))
                        ((IJavaScriptObject)this).Invoke(EngineProperties.JAVASCRIPT_CREATE_CALLBACK, []);
					_javaScriptCanUpdate = ((IJavaScriptObject)this).Has(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK);
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
		if (((IJavaScriptObject)this).Has(EngineProperties.JAVASCRIPT_ACTIVE_CALLBACK))
            ((IJavaScriptObject)this).Invoke(EngineProperties.JAVASCRIPT_ACTIVE_CALLBACK, []);
	}

	~JavaScriptNode()
	{
		if (((IJavaScriptObject)this).Has(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK))
            ((IJavaScriptObject)this).Invoke(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK, []);
	}

	public override void _Process(double delta)
	{
		if (_javaScriptCanUpdate)
            ((IJavaScriptObject)this).Invoke(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, [delta]);
	}
}
