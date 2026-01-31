using Godot;
using Jint.Native;
using System;
using System.Collections.Generic;


#region 包装基类
abstract partial class BaseEncounterRegisterData
{
	public virtual BaseEncounter GetInstance()
	{
		return null;
	}
}
abstract partial class BaseItemRegisterData
{
	public virtual BaseItem GetInstance()
	{
		return null;
	}
}
abstract partial class BaseEnemyRegisterData
{
	public virtual BaseEnemy GetInstance()
	{
		return null;
	}
}
#endregion

partial class EncounterRegisterData : BaseEncounterRegisterData
{
	private Type _encounterType;
	public EncounterRegisterData(Type encounterType)
	{
		if (encounterType == null)
			UtmxLogger.Error(nameof(encounterType),
				TranslationServer.Translate("The registered type name cannot be empty"));

		if (encounterType.IsAbstract || encounterType.IsInterface)
			UtmxLogger.Error($"{encounterType.Name} {
				TranslationServer.Translate("is an abstract class or interface and cannot be instantiated.")}");

		if (!typeof(BaseEncounter).IsAssignableFrom(encounterType))
			UtmxLogger.Error($"{encounterType.Name} {
				TranslationServer.Translate("Cannot register because it is not a subclass of the specified type")}");
		_encounterType = encounterType;
	}
	public override BaseEncounter GetInstance()
	{
		try
		{
			object instance = Activator.CreateInstance(_encounterType);
			return instance as BaseEncounter;
		}
		catch (MissingMethodException ex)
		{
			UtmxLogger.Error($"{
				TranslationServer.Translate("Failed to instantiate object")} {_encounterType.Name}: {
				TranslationServer.Translate("Missing public constructor")}: {ex.Message}");
		}
		catch (Exception ex)
		{
			UtmxLogger.Error($"{
				TranslationServer.Translate("Failed to instantiate object")} {_encounterType.Name}: {ex.Message}");
		}
		return null;
	}
}
partial class EnemyRegisterData : BaseEnemyRegisterData
{
	private Type _enemyType;
	public EnemyRegisterData(Type enemyType)
	{
		if (enemyType == null)
			throw new ArgumentNullException(nameof(enemyType), 
				TranslationServer.Translate("The registered type name cannot be empty"));

		if (enemyType.IsAbstract || enemyType.IsInterface)
			UtmxLogger.Error($"{enemyType.Name} {
				TranslationServer.Translate("is an abstract class or interface and cannot be instantiated.")}");

		if (!typeof(BaseEnemy).IsAssignableFrom(enemyType))
			UtmxLogger.Error($"{enemyType.Name} {
				TranslationServer.Translate("Cannot register because it is not a subclass of the specified type")}");
		_enemyType = enemyType;
	}
	public override BaseEnemy GetInstance()
	{
		try
		{
			object instance = Activator.CreateInstance(_enemyType);
			return instance as BaseEnemy;
		}
		catch (MissingMethodException ex)
		{
			UtmxLogger.Error($"{
				TranslationServer.Translate("Failed to instantiate object")} {_enemyType.Name}: {
				TranslationServer.Translate("Missing public constructor")}: {ex.Message}");
		}
		catch (Exception ex)
		{
			UtmxLogger.Error($"{
				TranslationServer.Translate("Failed to instantiate object")} {_enemyType.Name}: {ex.Message}");
		}
		return null;
	}
}
partial class ItemRegisterData : BaseItemRegisterData
{
	private Type _itemType;
	public ItemRegisterData(Type itemType)
	{
		if (itemType == null)
			throw new ArgumentNullException(nameof(itemType),
				TranslationServer.Translate("The registered type name cannot be empty"));
		
		if (itemType.IsAbstract || itemType.IsInterface)
			UtmxLogger.Error($"{itemType.Name} {
				TranslationServer.Translate("is an abstract class or interface and cannot be instantiated.")}");
		
		if (!typeof(BaseItem).IsAssignableFrom(itemType))
			UtmxLogger.Error($"{itemType.Name} {
				TranslationServer.Translate("Cannot register because it is not a subclass of the specified type")}");
		_itemType = itemType;
	}
	public override BaseItem GetInstance()
	{
		try
		{
			object instance = Activator.CreateInstance(_itemType);
			return instance as BaseItem;
		}
		catch (MissingMethodException ex)
		{
			UtmxLogger.Error($"{
				TranslationServer.Translate("Failed to instantiate item object")} {_itemType.Name}: {
				TranslationServer.Translate("Missing public constructor")}: {ex.Message}");
		}
		catch (Exception ex)
		{
			UtmxLogger.Error($"{
				TranslationServer.Translate("Failed to instantiate object")} {_itemType.Name}: {ex.Message}");
		}
		return null;
	}
}

#region JavaScript注册类

partial class JavaScriptEncounterRegisterData : BaseEncounterRegisterData
{
	private string _jsPath;
	public JavaScriptEncounterRegisterData(string scriptPath)
	{
		_jsPath = scriptPath;
	}
	public override BaseEncounter GetInstance()
	{
		return IJavaScriptObject.New<JavaScriptEncounterProxy>(_jsPath);
	}
}
partial class JavaScriptItemRegisterData : BaseItemRegisterData
{
	private string _jsPath;
	public JavaScriptItemRegisterData(string scriptPath)
	{
		_jsPath = scriptPath;
	}
	public override BaseItem GetInstance()
	{
		return IJavaScriptObject.New<JavaScriptItemProxy>(_jsPath);
	}
}
partial class JavaScriptEnemyRegisterData : BaseEnemyRegisterData
{
	private string _jsPath;
	public JavaScriptEnemyRegisterData(string scriptPath)
	{
		_jsPath = scriptPath;
	}
	public override BaseEnemy GetInstance()
	{
		return IJavaScriptObject.New<JavaScriptEnemyProxy>(_jsPath);
	}
}

#endregion

public partial class GameRegisterDB
{

	private static Dictionary<string, BaseEncounterRegisterData> _encounterDB = new();
	private static Dictionary<string, BaseItemRegisterData> _itemDB = new();
	private static Dictionary<string, BaseEnemyRegisterData> _enemyDB = new();

	~GameRegisterDB()
	{
		_encounterDB.Clear();
		_itemDB.Clear();
		_enemyDB.Clear();
	}


	public static void RegisterEncounter(string encounterId, Type t)
	{
		if (string.IsNullOrEmpty(encounterId))
			UtmxLogger.Warning(TranslationServer.Translate("The registered object's ID is invalid and cannot be empty."));
		_encounterDB.Add(encounterId, new EncounterRegisterData(t));
	}
	public static void RegisterEncounter(string encounterId, string scriptPath)
	{
		if (string.IsNullOrEmpty(encounterId))
			UtmxLogger.Warning(TranslationServer.Translate("The registered object's ID is invalid and cannot be empty."));
		scriptPath = UtmxResourceLoader.ResolvePath(scriptPath);
		_encounterDB.Add(encounterId, new JavaScriptEncounterRegisterData(scriptPath));
	}
	public static bool TryGetEncounter(string encounterId, out BaseEncounter encounter)
	{
		encounter = null;
		if (_encounterDB.TryGetValue(encounterId, out BaseEncounterRegisterData encounterData))
		{
			encounter = encounterData.GetInstance();
			return true;
		}
		else
		{
			UtmxLogger.Warning($"{
				TranslationServer.Translate("Invalid")} '{encounterId}' {
				TranslationServer.Translate("Please check if the object has completed registration.")}");
		}
		return false;
	}

	public static void RegisterEnemy(string enemyId, Type t)
	{
		if (string.IsNullOrEmpty(enemyId))
			UtmxLogger.Warning(TranslationServer.Translate("The registered object's ID is invalid and cannot be empty."));
		_enemyDB.Add(enemyId, new EnemyRegisterData(t));
	}
	public static void RegisterEnemy(string enemyId, string scriptPath)
	{
		if (string.IsNullOrEmpty(enemyId))
			UtmxLogger.Warning(TranslationServer.Translate("The registered object's ID is invalid and cannot be empty."));
		scriptPath = UtmxResourceLoader.ResolvePath(scriptPath);
		_enemyDB.Add(enemyId, new JavaScriptEnemyRegisterData(scriptPath));
	}
	public static bool TryGetEnemy(string enemyId, out BaseEnemy enemy)
	{
		enemy = null;
		if (_enemyDB.TryGetValue(enemyId, out BaseEnemyRegisterData enemyData))
		{
			enemy = enemyData.GetInstance();
			return true;
		}
		else
		{
			UtmxLogger.Warning($"{
				TranslationServer.Translate("Invalid")} '{enemyId}' {
				TranslationServer.Translate("Please check if the object has completed registration.")}");
		}
		return false;
	}
	public static void RegisterItem(string itemId, Type t)
	{
		if (string.IsNullOrEmpty(itemId))
			UtmxLogger.Warning(TranslationServer.Translate("The registered object's ID is invalid and cannot be empty."));
		_itemDB.Add(itemId, new ItemRegisterData(t));
	}
	public static void RegisterItem(string itemId, string scriptPath)
	{
		if (string.IsNullOrEmpty(itemId))
			UtmxLogger.Warning(TranslationServer.Translate("The registered object's ID is invalid and cannot be empty."));
		scriptPath = UtmxResourceLoader.ResolvePath(scriptPath);
		_itemDB.Add(itemId, new JavaScriptItemRegisterData(scriptPath));
	}
	public static bool TryGetItem(string itemId, out BaseItem item)
	{
		item = null;
		if (_itemDB.TryGetValue(itemId, out BaseItemRegisterData itemData))
		{
			item = itemData.GetInstance();
			return true;
		}
		else
		{
			UtmxLogger.Warning($"{
				TranslationServer.Translate("Invalid")} '{itemId}' {
				TranslationServer.Translate("Please check if the object has completed registration.")}");
		}
		return false;
	}

}
