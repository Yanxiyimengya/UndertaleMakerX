using Godot;
using Jint.Native;

// 这是表示一个脚本实例的对象
// 任何附带脚本的对象都必须继承于他
public abstract partial class ScriptClass : RefCounted
{
    public abstract ScriptObjectInstance New();
}

