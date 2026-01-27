using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract partial class ScriptObjectInstance : RefCounted
{
    public abstract object Get(string key);
    public abstract void Set(string key, object value);
}

