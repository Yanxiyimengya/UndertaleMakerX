using Godot;
using System;

[GlobalClass]
public partial class SceneManager : CanvasLayer
{

	[Export(PropertyHint.File, "*.tscn")]
	public string EncounterBattleScenePath = "";
	[Export(PropertyHint.File, "*.tscn")]
	public string GameoverScenePath = "";

	public static SceneManager Instance { get; private set; }
	
	public override void _EnterTree()
	{
		Instance = this;
	}

	public void ChangeSceneToFile(string filePath)
	{
		GetTree()?.ChangeSceneToFile(filePath);
	}
	
}
