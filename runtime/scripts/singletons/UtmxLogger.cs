using Godot;
using System;

static class UtmxLogger
{
    public static void Log(params object[] message)
    {
        GD.Print(message);
    }
}
