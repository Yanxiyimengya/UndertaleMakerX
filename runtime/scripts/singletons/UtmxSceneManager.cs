using Godot;
using System;

[GlobalClass]
public partial class UtmxSceneManager : CanvasLayer
{
	[Export(PropertyHint.File, "*.tscn")]
	public string EncounterBattleScenePath = "";
	[Export(PropertyHint.File, "*.tscn")]
	public string GameoverScenePath = "";

	private string _prevScene = "";
	private string _currentScene = "";
	private string _mainScene = "";

	public static UtmxSceneManager Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
		_mainScene = UtmxResourceLoader.ResolvePath(
			(string)UtmxRuntimeProjectConfig.TryGetDefault("application/main_scene", string.Empty)
		);
	}

	public override void _ExitTree()
	{
		Instance = null;
	}

	public void ChangeSceneToFile(string filePath)
	{
		if (!FileAccess.FileExists(filePath))
		{
			UtmxLogger.Error(TranslationServer.Translate("Failed to switch scene: Invalid scene path."));
			return;
		}
		_prevScene = _currentScene;
		_currentScene = filePath;
		filePath = UtmxResourceLoader.ResolvePath(filePath);
		GetTree()?.ChangeSceneToFile(filePath);
	}

	public string GetMainScenePath()
	{
		return _mainScene;
	}
}
