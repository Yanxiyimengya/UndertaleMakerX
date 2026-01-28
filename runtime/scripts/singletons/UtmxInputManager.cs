using Godot;
using System;

[GlobalClass]
public partial class UtmxInputManager : Node
{
    public static UtmxInputManager Instance;
    public override void _EnterTree()
    {
        Instance = this;
    }
    public static bool IsActionPressed(string action)
    {
        return Input.IsActionPressed(action);
    }

    public static bool IsActionJustPressed(string action)
    {
        return Input.IsActionJustPressed(action);
    }

    public static bool IsActionJustReleased(string action)
    {
        return Input.IsActionJustReleased(action);
    }

    public static bool IsKeyPressed(long keycode)
    {
        return Input.IsKeyPressed((Key)keycode);
    }

    public static bool IsPhysicalKeyPressed(long scancode)
    {
        return Input.IsPhysicalKeyPressed((Key)scancode);
    }

    public static bool IsMouseButtonPressed(long button)
    {
        return Input.IsMouseButtonPressed((MouseButton)button);
    }

    public Vector2? GetMouseGlobalPosition()
    {
        Viewport viewport = GetViewport();
        if (viewport != null)
            return viewport.GetMousePosition();
        return null;
    }
}
