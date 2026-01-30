using Godot;
using System;

[GlobalClass]
public partial class UtmxGameManager : Node
{
	public static UtmxGameManager Instance;
	private JavaScriptObjectInstance _mainScriptInstace;

	public override void _Ready()
	{
		Instance = this;
		string mainScriptFilePath = UtmxRuntimeProjectConfig.TryGetDefault("application/main_script", string.Empty).ToString();
		mainScriptFilePath = UtmxResourceLoader.ResolvePath(mainScriptFilePath);
		if (FileAccess.FileExists(mainScriptFilePath))
		{
			_mainScriptInstace = JavaScriptBridge.FromFile(mainScriptFilePath)?.New();
		}
	}

	public override void _ExitTree()
	{
		_GameEnd();
		Instance = null;
		_mainScriptInstace = null;
	}


	public void _GameStart()
	{
		GameRegisterDB.RegisterEnemy("BaseEnemy", typeof(BaseEnemy));
		GameRegisterDB.RegisterEnemy("MyEnemy", "js/test_js_enemy.js");
		GameRegisterDB.RegisterItem("BaseItem", typeof(BaseItem));
		GameRegisterDB.RegisterItem("MyItem", "js/test_js_item.js");


		UtmxPlayerDataManager.AddItem("BaseItem");
		UtmxPlayerDataManager.AddItem("MyItem");
		UtmxBattleManager.Instance.EncounterBattleStart(new BaseEncounterConfiguration());
		_mainScriptInstace?.Invoke("onGameStart", []);

	}

	public void _GameEnd()
	{
		_mainScriptInstace?.Invoke("onGameEnd", []);
	}
}
