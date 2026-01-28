using Godot;

static class UtmxLogger
{
    public static void Log(params object[] message)
    {
        GD.Print(message);
    }
    public static void Warning(params object[] message)
    {
        GD.PushWarning(message);
    }

    public static void Error(params object[] message)
    {
        GD.PrintErr(message);
    }
}
