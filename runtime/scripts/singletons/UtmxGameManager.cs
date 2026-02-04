using Godot;
using Jint.Native.Object;
using System;

[GlobalClass]
public partial class UtmxGameManager : Node
{
	[Signal]
	public delegate void GameStartEventHandler();
	[Signal]
	public delegate void GameEndEventHandler();

	public static UtmxGameManager Instance;

	private static JavaScriptGameManagerBoot boot;

	public override void _EnterTree()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
		boot = new JavaScriptGameManagerBoot();
	}
	public override void _ExitTree()
	{
		Instance = null;
		_GameEnd();
	}

	public void _GameStart()
	{
		EmitSignal(SignalName.GameStart, []);

	}
	public void _GameEnd()
	{
		EmitSignal(SignalName.GameEnd, []);
	}


	public static void QuitGame()
	{
		Instance.GetTree().Quit();
	}

	public static double GetFpsReal() { return Engine.GetFramesPerSecond(); }
	public static void SetMaxFps(int fps) { Engine.MaxFps = fps; }

}
