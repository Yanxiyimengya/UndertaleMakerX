using Godot;

[GlobalClass]
public partial class UtmxMainLoop : SceneTree
{
    private Godot.Collections.Dictionary<string, string> _cmdArgs = new Godot.Collections.Dictionary<string, string>();

    public UtmxMainLoop()
    {
        _cmdArgs = ParseCmdlineArgs();
        if (_cmdArgs.TryGetValue("pack", out string value))
        {
            ProjectSettings.LoadResourcePack(value, true, 0); // 命令行指定资源包
        }
        UtmxLogger.Log($"UndertaleMakerX {EngineProperties.ENGINE_VERSION} - Yanxiyimeng");
        // 加载资源包
        var datapackLoader = DatapackLoader.GetDatapackLoader(OS.GetName());
        if (datapackLoader != null)
            datapackLoader.LoadPack();
    }
    public override void _Initialize()
    {
        // 加载资源包配置项
        UtmxRuntimeProjectConfig.Loadencounter($"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/project_config.json");

        InitializeWindow();

        Engine.MaxFps = UtmxRuntimeProjectConfig.TryGetDefault("application/max_fps",
            ProjectSettings.GetSetting("application/run/max_fps")).AsInt32();

        DisplayServer.VSyncMode vsyncMode = UtmxRuntimeProjectConfig.TryGetDefault("application/vsync",
            DisplayServer.WindowGetVsyncMode() != DisplayServer.VSyncMode.Disabled)
            ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
        DisplayServer.WindowSetVsyncMode(vsyncMode);


        Color clearColor = Color.FromString(
            (string)UtmxRuntimeProjectConfig.TryGetDefault("window/clear_color", ""),
             ProjectSettings.GetSetting("rendering/environment/defaults/default_clear_color").AsColor());
        RenderingServer.SetDefaultClearColor(clearColor);
    }

    public override bool _Process(double delta)
    {
        return false;
    }

    public override void _Finalize()
    {
        _cmdArgs.Clear();
    }


    private Godot.Collections.Dictionary<string, string> ParseCmdlineArgs()
    {

        var arguments = new Godot.Collections.Dictionary<string, string>();
        string[] cmdLineArgs = OS.GetCmdlineArgs();
        foreach (string arg in cmdLineArgs)
        {
            string cleanArg = arg.StartsWith("--") ? arg.Substring(2) : arg;

            if (cleanArg.Contains("="))
            {
                string[] keyValue = cleanArg.Split('=', 2);
                string key = keyValue[0].Trim();
                string value = keyValue.Length > 1 ? keyValue[1].Trim() : string.Empty;
                if (!arguments.ContainsKey(key))
                {
                    arguments[key] = value;
                }
            }
            else
            {
                string key = cleanArg.Trim();
                if (!string.IsNullOrEmpty(key) && !arguments.ContainsKey(key))
                {
                    arguments[key] = string.Empty;
                }
            }
        }
        return arguments;
    }

    private void InitializeWindow()
    {
        Vector2I windowSize = new Vector2I(
            UtmxRuntimeProjectConfig.TryGetDefault("window/width",
                ProjectSettings.GetSetting("display/window/size/viewport_width")).AsInt32(),
            UtmxRuntimeProjectConfig.TryGetDefault("window/height",
                ProjectSettings.GetSetting("display/window/size/viewport_height")).AsInt32());

        bool resizable = UtmxRuntimeProjectConfig.TryGetDefault<bool>(
            "window/resizable",
            (bool)ProjectSettings.GetSetting("display/window/size/resizable")
            );
        bool fullscreen = UtmxRuntimeProjectConfig.TryGetDefault<bool>(
            "window/fullscreen",
            (ProjectSettings.GetSetting("display/window/size/mode").AsInt32() == (uint)DisplayServer.WindowMode.Fullscreen)
            );
        bool boderless = UtmxRuntimeProjectConfig.TryGetDefault<bool>(
            "window/boderless",
            (ProjectSettings.GetSetting("display/window/size/borderless").AsBool())
            );

        string appName = UtmxRuntimeProjectConfig.TryGetDefault("application/name",
                ProjectSettings.GetSetting("application/config/name")).AsString();

        Root.Connect(Window.SignalName.Ready, Callable.From(delegate ()
        {
            int currentScreen = DisplayServer.WindowGetCurrentScreen();
            Rect2I screenRect = DisplayServer.ScreenGetUsableRect(currentScreen);
            Vector2I centerPosition = screenRect.Position + (screenRect.Size - windowSize) / 2;
            Root.Size = windowSize;
            Root.Position = centerPosition;
            Root.Borderless = boderless;

            Root.Unresizable = !resizable;
            if (!string.IsNullOrEmpty(appName))
                Root.Title = appName;
            if (fullscreen)
                Root.Mode = Window.ModeEnum.Fullscreen;
        }), (int)GodotObject.ConnectFlags.OneShot);
        ProjectSettings.SetSetting("application/config/name", appName);
        ProjectSettings.SetSetting("display/window/size/viewport_width", windowSize.X);
        ProjectSettings.SetSetting("display/window/size/viewport_height", windowSize.Y);
    }
}
