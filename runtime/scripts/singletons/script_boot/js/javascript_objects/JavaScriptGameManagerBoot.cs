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
        GD.Print("主脚本路径", mainScriptFilePath);
        mainScriptFilePath = UtmxResourceLoader.ResolvePath(mainScriptFilePath);
        if (FileAccess.FileExists(mainScriptFilePath))
        {
            JavaScriptClass jsClass = JavaScriptBridge.FromFile(mainScriptFilePath);
            _mainScriptObject = jsClass?.New();
        }
        GD.Print("主脚本实例包含onGameStart？", _mainScriptObject?.HasProperty("onGameStart"));
        GD.Print("主脚本实例包含onGameEnd？", _mainScriptObject?.HasProperty("onGameEnd"));
        if (_mainScriptObject?.HasProperty("onGameStart") == true)
            UtmxGameManager.Instance.Connect(UtmxGameManager.SignalName.GameStart, Callable.From(_OnGameStart));
        if (_mainScriptObject?.HasProperty("onGameEnd") == true)
            UtmxGameManager.Instance.Connect(UtmxGameManager.SignalName.GameEnd, Callable.From(_OnGameEnd));
        UtmxGameManager.Instance.AddChild(new JavaScriptTweenManager());
    }
    public void _OnGameStart()
    {
         JavaScriptBridge.InvokeFunction(_mainScriptObject, "onGameStart", []);
    }
    public void _OnGameEnd()
    {
         JavaScriptBridge.InvokeFunction(_mainScriptObject, "onGameEnd", []);
    }
}
