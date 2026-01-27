using Godot;
using System;

[GlobalClass]
public partial class JavaScriptNode : Node
{
	[Export(PropertyHint.FilePath, "*.js")]
	public string JavaScriptFile;
	private JavaScriptObjectInstance _instance;
	public override void _EnterTree()
	{
		if (! string.IsNullOrEmpty(JavaScriptFile))
		{
			JavaScriptClass javaScriptClass = ScriptBoot.Instance.GetBridge<JavaScriptBridge>().FromFile(JavaScriptFile);
			_instance = javaScriptClass.New();
		}
	}

	public override void _Ready()
	{
		base._Ready();
		_instance?.Invoke("start", []);
	}

	~JavaScriptNode()
	{
		_instance?.Invoke("destroy", []);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		_instance?.Invoke("update", []);
	}
}
