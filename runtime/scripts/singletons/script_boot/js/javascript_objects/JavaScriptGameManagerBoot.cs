using Godot;
using Jint.Native.Object;
using System;
using System.Collections.Generic;
public partial class JavaScriptGameManagerBoot : Node
{
    private static ObjectInstance _mainScriptObject;
    public JavaScriptGameManagerBoot()
    {
        string mainScriptFilePath = UtmxRuntimeProjectConfig.TryGetDefault("application/main_script", string.Empty).ToString();
        mainScriptFilePath = UtmxResourceLoader.ResolvePath(mainScriptFilePath);
        if (FileAccess.FileExists(mainScriptFilePath))
        {
            JavaScriptClass jsClass = JavaScriptBridge.FromFile(mainScriptFilePath);
            _mainScriptObject = jsClass?.New();
        }
        UtmxGameManager.Instance.Connect(UtmxGameManager.SignalName.GameStart, Callable.From(OnGameStart));
        UtmxGameManager.Instance.Connect(UtmxGameManager.SignalName.GameEnd, Callable.From(OnGameEnd));
        UtmxGameManager.Instance.AddChild(new JavaScriptTweenManager());
    }
    public static void OnGameStart()
    {
        JavaScriptBridge.InvokeFunction(_mainScriptObject, "onGameStart", []);
    }
    public static void OnGameEnd()
    {
        JavaScriptBridge.InvokeFunction(_mainScriptObject, "onGameEnd", []);
    }
}
