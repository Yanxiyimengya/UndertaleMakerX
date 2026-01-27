using System;

public class ScriptBoot
{

    private static readonly Lazy<ScriptBoot> _instance =
        new Lazy<ScriptBoot>(() => new ScriptBoot());
    private ScriptBoot()
    {
        _bridge = new JavaScriptBridge();
    }
    public static ScriptBoot Instance => _instance.Value;


    private ScriptBridge _bridge;

    public T GetBridge<T>() where T : ScriptBridge
    {
        return (T)_bridge;
    }
}
