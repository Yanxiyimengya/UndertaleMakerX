// 与脚本语言的桥接层
// 若希望自定义脚本语言，必须实现以下方法
public abstract class ScriptBridge
{
    public abstract ScriptClass ExecuteString(string code);
    public abstract object GetValue(string value);
    public abstract void SetValue(string valueName, object value);
}
