using Godot;

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
			if (!string.IsNullOrEmpty(_javaScriptFile))
			{
				JavaScriptClass javaScriptClass = ScriptBoot.Instance.GetBridge<JavaScriptBridge>().FromFile(_javaScriptFile);
				if (javaScriptClass != null)
				{
					_instance = javaScriptClass.New();
				}
			}
		}
	}

	private JavaScriptObjectInstance _instance;
	public string _javaScriptFile;
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
		_instance?.Invoke("update", [delta]);
	}
}
