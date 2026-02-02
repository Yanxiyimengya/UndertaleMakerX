
using Godot;
using System.Collections.Generic;

// 用于管理场景中动态添加的节点的单例
public partial class UtmxNodeManager : Node
{
	public UtmxNodeManager Instance;


	public override void _EnterTree()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
	}

	public override void _ExitTree()
	{
		Instance = null;
	}

	private static Queue<Sprite2D> spritePool = new();
	public static void CreateSprite<T>(string texturePath = "") where T : Sprite2D, new()
    {
	}
}
