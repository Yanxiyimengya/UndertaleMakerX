using Godot;
using System;

public class JavaScriptGlobalInterface
{
    public static void Print(params object[] args)
    {
        UtmxLogger.Log(args);
    }
}
