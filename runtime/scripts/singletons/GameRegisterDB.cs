using Godot;
using Jint.Native;
using System;
using System.Collections.Generic;


#region 包装基类
abstract partial class BaseEncounterRegisterData
{
	public virtual BaseEncounterConfiguration GetInstance()
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
			throw new ArgumentNullException(nameof(encounterType), "EncounterConfiguration Type cannot be null");
		if (encounterType.IsAbstract || encounterType.IsInterface)
			throw new ArgumentException($"{encounterType.Name} is an abstract class or interface and cannot be instantiated",
				nameof(encounterType));
		if (!typeof(BaseEncounterConfiguration).IsAssignableFrom(encounterType))
			throw new ArgumentException($"{encounterType.Name} is not a subclass of BaseEncounterConfiguration and cannot be registered", nameof(encounterType));
		_encounterType = encounterType;
	}
	public override BaseEncounterConfiguration GetInstance()
	{
		try
		{
			object instance = Activator.CreateInstance(_encounterType);
			return instance as BaseEncounterConfiguration;
		}
		catch (MissingMethodException ex)
		{
			UtmxLogger.Error($"Failed to instantiate EncounterConfiguration {_encounterType.Name}: No public parameterless constructor found! {ex.Message}");
		}
		catch (Exception ex)
		{
			UtmxLogger.Error($"Failed to instantiate EncounterConfiguration {_encounterType.Name}: {ex.Message}");
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
			throw new ArgumentNullException(nameof(enemyType), "Enemy Type cannot be null");
		if (enemyType.IsAbstract || enemyType.IsInterface)
			throw new ArgumentException($"{enemyType.Name} is an abstract class or interface and cannot be instantiated", nameof(enemyType));
		if (!typeof(BaseEnemy).IsAssignableFrom(enemyType))
			throw new ArgumentException($"{enemyType.Name} is not a subclass of BaseEnemy and cannot be registered", nameof(enemyType));
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
			UtmxLogger.Error($"Failed to instantiate enemy {_enemyType.Name}: No public parameterless constructor found! {ex.Message}");
		}
		catch (Exception ex)
		{
			UtmxLogger.Error($"Failed to instantiate enemy {_enemyType.Name}: {ex.Message}");
		}
		return null;
	}
}
partial class ItemRegisterData : BaseItemRegisterData
{
	private Type _enemyType;
	public ItemRegisterData(Type itemType)
	{
		if (itemType == null)
			throw new ArgumentNullException(nameof(itemType), "Item Type cannot be null");
		if (itemType.IsAbstract || itemType.IsInterface)
			throw new ArgumentException($"{itemType.Name} is an abstract class or interface and cannot be instantiated", nameof(itemType));
		if (!typeof(BaseItem).IsAssignableFrom(itemType))
			throw new ArgumentException($"{itemType.Name} is not a subclass of BaseItem and cannot be registered", nameof(itemType));
		_enemyType = itemType;
	}
	public override BaseItem GetInstance()
	{
		try
		{
			object instance = Activator.CreateInstance(_enemyType);
			return instance as BaseItem;
		}
		catch (MissingMethodException ex)
		{
			UtmxLogger.Error($"Failed to instantiate item {_enemyType.Name}: No public parameterless constructor found! {ex.Message}");
		}
		catch (Exception ex)
		{
			UtmxLogger.Error($"Failed to instantiate item {_enemyType.Name}: {ex.Message}");
		}
		return null;
	}
}

#region JavaScript注册类

partial class JavaScriptEncounterRegisterData : BaseEncounterRegisterData
{
	private string _jsPath;
	private JavaScriptClass _jsClass;
	public JavaScriptEncounterRegisterData(string scriptPath)
	{
		_jsPath = scriptPath;
		_jsClass = JavaScriptBridge.FromFile(_jsPath);
	}
	public override BaseEncounterConfiguration GetInstance()
	{
		if (_jsClass != null)
		{
			JavaScriptObjectInstance instance = _jsClass.New();
			return instance.ToObject() as BaseEncounterConfiguration;
		}
		return null;
	}
}
partial class JavaScriptItemRegisterData : BaseItemRegisterData
{
	private string _jsPath;
	private JavaScriptClass _jsClass;
	public JavaScriptItemRegisterData(string scriptPath)
	{
		_jsPath = scriptPath;
		_jsClass = JavaScriptBridge.FromFile(_jsPath);
		if (_jsClass == null)
		{
			UtmxLogger.Error($"Unable to load script from {scriptPath}");
		}
	}
	public override BaseItem GetInstance()
	{
		JavaScriptObjectInstance instance = _jsClass?.New();
		if (instance != null)
		{
			JavaScriptItemProxy item = instance.ToObject() as JavaScriptItemProxy;
			item.JsInstance = instance;
			return item;
		}
		return null;
	}
}
partial class JavaScriptEnemyRegisterData : BaseEnemyRegisterData
{
	private string _jsPath;
	private JavaScriptClass _jsClass;
	public JavaScriptEnemyRegisterData(string scriptPath)
	{
		_jsPath = scriptPath;
		_jsClass = JavaScriptBridge.FromFile(_jsPath);
		if (_jsClass == null)
		{
			UtmxLogger.Error($"Unable to load script from {scriptPath}");
		}
	}
	public override BaseEnemy GetInstance()
	{
		JavaScriptObjectInstance instance = _jsClass?.New();
		if (instance != null)
		{
			JavaScriptEnemyProxy enemy = instance.ToObject() as JavaScriptEnemyProxy;
			enemy.JsInstance = instance;
			return enemy;
		}
		return null;
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

	public static void RegisterEnemy(string enemyId, Type t)
	{
		_enemyDB.Add(enemyId, new EnemyRegisterData(t));
	}
	public static void RegisterEnemy(string enemyId, string scriptPath)
	{
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
			UtmxLogger.Warning($"EnemyId '{enemyId}' not found in GameRegisterDB.");
		}
		return false;
	}
	public static void RegisterItem(string itemId, Type t)
	{
		_itemDB.Add(itemId, new ItemRegisterData(t));
	}
	public static void RegisterItem(string itemId, string scriptPath)
	{
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
			UtmxLogger.Warning($"itemId '{itemId}' not found in GameRegisterDB.");
		}
		return false;
	}

}
