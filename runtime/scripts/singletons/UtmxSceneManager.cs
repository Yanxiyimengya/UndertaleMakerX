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
	private string _currentScene = ProjectSettings.GetSetting("application/run/main_scene", "").ToString();
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
		_prevScene = _currentScene;
		_currentScene = filePath;
		filePath = UtmxResourceLoader.ResolvePath(filePath);
		if (! ResourceLoader.Exists(filePath))
		{
			UtmxLogger.Error(TranslationServer.Translate("Failed to switch scene: Invalid scene path:"), $": {filePath}");
			return;
		}
		if ( !filePath.EndsWith(".tscn") && !filePath.StartsWith("uid://") )
			filePath += ".tscn";
		GetTree()?.ChangeSceneToFile(filePath);
	}

	public string GetMainScenePath()
	{
		return _mainScene;
	}
	public string GetCurrentScenePath()
	{
		return _currentScene;
	}
	public string GetPreviousScenePath()
	{
		return _prevScene;
	}


    #region 渲染对象管理

    private static ObjectPool<DrawableObject> _drawableObjectPool = new();
    public static DrawableObject CreateDrawableObject()
    {
        return CreateDrawableObject<DrawableObject>();
    }
    public static T CreateDrawableObject<T>() where T : DrawableObject, new()
    {
        T node = _drawableObjectPool.GetObject<T>();
		Node parent = node.GetParent();
        Node targetParent = Instance.GetTree().CurrentScene;
        if (parent == null) targetParent.CallDeferred("add_child", node);
        else if (parent != targetParent) targetParent.CallDeferred("reparent", node);
        return node;
    }

    public static void DeleteDrawableObject(DrawableObject typer)
    {
        if (typer == null) return;
        _drawableObjectPool.DisabledObject(typer);
    }
    #endregion

    #region 精灵管理

    private static ObjectPool<GameSprite2D> _spritePool = new(); 
	public static GameSprite2D CreateSprite()
	{
		return CreateSprite<GameSprite2D>();
	}
    public static T CreateSprite<T>() where T : GameSprite2D, new()
    {
        T node = _spritePool.GetObject<T>();
        Node parent = node.GetParent();
        Node targetParent = Instance.GetTree().CurrentScene;
        if (parent == null) targetParent.CallDeferred("add_child", node);
        else if (parent != targetParent) targetParent.CallDeferred("reparent", node);
        return node;
    }
    public static void DeleteSprite(GameSprite2D sprite)
	{
		if (sprite == null) return;
		_spritePool.DisabledObject(sprite);
	}
	#endregion

	#region 打字机管理

	private static ObjectPool<TextTyper> _textTyperPool = new();
	public static TextTyper CreateTextTyper()
	{
		return CreateTextTyper<TextTyper>();
	}
	public static T CreateTextTyper<T>() where T : TextTyper, new()
    {
        T node = _textTyperPool.GetObject<T>();
        Node parent = node.GetParent();
        Node targetParent = Instance.GetTree().CurrentScene;
        if (parent == null) targetParent.CallDeferred("add_child", node);
        else if (parent != targetParent) targetParent.CallDeferred("reparent", node);
        return node;
    }

	public static void DeleteTextTyper(TextTyper typer)
	{
		if (typer == null) return;
		_textTyperPool.DisabledObject(typer);
	}
	#endregion
}
