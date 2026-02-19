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
        if (_mainScriptObject?.HasProperty("onGameStart") == true)
            UtmxGameManager.Instance.Connect(UtmxGameManager.SignalName.GameStart, Callable.From(_OnGameStart));
        if (_mainScriptObject?.HasProperty("onGameEnd") == true)
            UtmxGameManager.Instance.Connect(UtmxGameManager.SignalName.GameEnd, Callable.From(_OnGameEnd));
        UtmxGameManager.Instance.AddChild(new JavaScriptTweenManager());
    }

    public override void _EnterTree()
    {
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
