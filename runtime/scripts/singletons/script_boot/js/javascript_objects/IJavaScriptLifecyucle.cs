using Godot;
using System;
public interface IJavaScriptLifecyucle : IJavaScriptObject
{
    public JavaScriptLifecycleProxy LifecycleProxy { get; set; }
}
