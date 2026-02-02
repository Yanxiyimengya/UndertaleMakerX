using Godot;
using Jint.Native.Object;
using System;

[GlobalClass]
public partial class UtmxGameManager : Node
{
	public static UtmxGameManager Instance;
	private IJavaScriptObject _mainScriptObject;

	public override void _Ready()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
		string mainScriptFilePath = UtmxRuntimeProjectConfig.TryGetDefault("application/main_script", string.Empty).ToString();
		mainScriptFilePath = UtmxResourceLoader.ResolvePath(mainScriptFilePath);
		if (FileAccess.FileExists(mainScriptFilePath))
		{
			_mainScriptObject = IJavaScriptObject.New<JavaScriptObject>(mainScriptFilePath);
		}
	}
	public override void _ExitTree()
	{
		_GameEnd();
		Instance = null;
		_mainScriptObject = null;
	}


	public void _GameStart()
	{
		_mainScriptObject?.Invoke("onGameStart", []);

	}
	public void _GameEnd()
	{
		_mainScriptObject?.Invoke("onGameEnd", []);
	}
	public static void QuitGame()
	{
		Instance.GetTree().Quit();
	}
}
