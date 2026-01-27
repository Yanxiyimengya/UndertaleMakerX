using System;

public class ScriptBoot
{
    private ScriptBridge _jsBridge;

    private static readonly Lazy<ScriptBoot> _instance =
        new Lazy<ScriptBoot>(() => new ScriptBoot());
    private ScriptBoot()
    {
        _jsBridge = new JavaScriptBridge();
    }
    public static ScriptBoot Instance => _instance.Value;

    public T GetBridge<T>() where T : ScriptBridge
    {
        return (T)_jsBridge;
    }
}
