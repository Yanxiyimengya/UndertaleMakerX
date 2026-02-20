using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

[GlobalClass]
public partial class UtmxSceneManager : CanvasLayer
{
	[Export(PropertyHint.File, "*.tscn")]
	public string EncounterBattleScenePath = "";
	[Export(PropertyHint.File, "*.tscn")]
	public string GameoverScenePath = "";
    [Export(PropertyHint.File, "*.tscn")]
    public string SpeechBubbleScenePath = "";
	[Export]
	public GameCamera MainCamera;

    private static string _prevScene = "";
	private static string _currentScene = ProjectSettings.GetSetting("application/run/main_scene", "").ToString();
	private static string _mainScene = "";
	private static Dictionary<string , Node> singletons = new();
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
		ProcessMode = ProcessModeEnum.Always;
	}

	public override void _ExitTree()
	{
		Instance = null;
		foreach (Node node in singletons.Values)
		{
			node.QueueFree();
		}
		singletons.Clear();
	}

	public static void ChangeSceneToFile(string filePath)
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
		Instance.GetTree()?.ChangeSceneToFile(filePath);
	}

	public static void AddSingleton(string name, Node node)
	{
		if (string.IsNullOrEmpty(name))
		{
			UtmxLogger.Error(TranslationServer.Translate("Cannot add singleton, singleton name is invalid:"), name);
			return;
		}
		if (singletons.ContainsKey(name))
		{
			UtmxLogger.Error(TranslationServer.Translate("Cannot add singleton, singleton already exists:"), name);
			return;
		}
		if (node != null && IsInstanceValid(node))
		{
			node.Name = name;
			if (node.GetParent() == null)
				 Instance.AddChild(node);
			else node.Reparent(Instance);
			singletons.Add(name, node);
		}
		else
		{
			UtmxLogger.Error(TranslationServer.Translate("Unable to add singleton, singleton object is invalid"), name);
			return;
		}
	}
	public static void RemoveSingleton(string name)
	{
		if (singletons.ContainsKey(name))
		{
			Node node = singletons[name];
			Instance.RemoveChild(node);
			if (IsInstanceValid(Instance?.GetTree()?.CurrentScene) && !node.IsQueuedForDeletion())
			{
				Instance.GetTree().CurrentScene.AddChild(node);
			}
		}
	}

	public static Node GetSingleton(string name)
	{
		if (singletons.ContainsKey(name))
		{
			return singletons[name];
		}
		return null;
	}

	public static string GetMainScenePath()
	{
		return _mainScene;
	}
	public static string GetCurrentScenePath()
	{
		return _currentScene;
	}
	public static string GetPreviousScenePath()
	{
		return _prevScene;
	}

    public static GameCamera GetCamera()
    {
        if (IsInstanceValid(Instance?.MainCamera))
            return Instance.MainCamera;
        if (Instance?.GetViewport()?.GetCamera2D() is GameCamera camera)
            return camera;
        return null;
    }


    #region 对话气泡管理
    public static SpeechBubble CreateSpeechBubble()
    {
        if (!IsInstanceValid(Instance))
            return null;

        Resource res = ResourceLoader.Load(Instance.SpeechBubbleScenePath);
		if (res is PackedScene sc)
		{
            SpeechBubble speechBubble = (SpeechBubble)sc.Instantiate();
            Node targetParent = Instance.GetTree()?.CurrentScene;
            if (IsInstanceValid(targetParent))
                targetParent.AddChild(speechBubble);
			return speechBubble;
        }
		return null;
    }
    public static void DeleteSpeechBubble(SpeechBubble obj)
    {
        if (obj == null) return;
        obj.QueueFree();
    }
    public static void DeleteDrawableObject(SpeechBubble obj)
    {
        DeleteSpeechBubble(obj);
    }
    #endregion

    #region 渲染对象管理
    private static ObjectPool<DrawableObject> _drawableObjectPool = new();
	public static DrawableObject CreateDrawableObject()
	{
		return CreateDrawableObject<DrawableObject>();
	}
	public static T CreateDrawableObject<T>() where T : DrawableObject, new()
	{
		T node = _drawableObjectPool.GetObject<T>();
		if (!IsInstanceValid(Instance))
			return node;

		Node parent = IsInstanceValid(node) ? node.GetParent() : null;
		Node targetParent = Instance.GetTree()?.CurrentScene;
        if (!IsInstanceValid(targetParent))
            return node;

        if (parent == null) targetParent.AddChild(node);
		else if (parent != targetParent && IsInstanceValid(parent)) node.Reparent(targetParent, false);
        else if (parent != targetParent) targetParent.AddChild(node);

		return node;
	}
	public static void DeleteDrawableObject(DrawableObject obj)
	{
		if (obj == null) return;
		_drawableObjectPool.DisabledObject(obj);
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
		if (!IsInstanceValid(Instance))
			return node;

		Node parent = IsInstanceValid(node) ? node.GetParent() : null;
		Node targetParent = Instance.GetTree()?.CurrentScene;
		if (!IsInstanceValid(targetParent))
			return node;

		if (parent == null) targetParent.AddChild(node);
		else if (parent != targetParent && IsInstanceValid(parent)) node.Reparent(targetParent, false);
		else if (parent != targetParent) targetParent.AddChild(node);
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
		if (!IsInstanceValid(Instance))
			return node;

		Node parent = IsInstanceValid(node) ? node.GetParent() : null;
		Node targetParent = Instance.GetTree()?.CurrentScene;
		if (!IsInstanceValid(targetParent))
			return node;

		if (parent == null) targetParent.AddChild(node);
		else if (parent != targetParent && IsInstanceValid(parent)) node.Reparent(targetParent, false);
		else if (parent != targetParent) targetParent.AddChild(node);
		return node;
	}

	public static void DeleteTextTyper(TextTyper typer)
	{
		if (typer == null) return;
		_textTyperPool.DisabledObject(typer);
	}
	#endregion
}
