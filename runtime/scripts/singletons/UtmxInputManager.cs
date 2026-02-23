using Godot;
using System;
using static System.Runtime.CompilerServices.RuntimeHelpers;

[GlobalClass]
public partial class UtmxInputManager : Node2D
{
    public static UtmxInputManager Instance;
    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }
    public override void _ExitTree()
    {
        Instance = null;
    }

    public static void AddAction(string actionName)
    {
        InputMap.AddAction(actionName);
    }
    public static bool HasAction(string actionName)
    {
        return InputMap.HasAction(actionName);
    }
    public static void EraseAction(string actionName)
    {
        InputMap.EraseAction(actionName);
    }
    public static bool ActionAddKeyButton(string actionName, Key keyButton)
    {
        if (UtmxInputManager.HasAction(actionName))
        {
            InputEventKey keyEvent = new();
            keyEvent.Keycode = keyButton;
            InputMap.ActionAddEvent(actionName, keyEvent);
            return true;
        }
        return false;
    }
    public static bool ActionAddKeyButton(string actionName, double keyButton)
    {
        return ActionAddKeyButton(actionName, (Key)keyButton);
    }
    public static bool ActionAddKeyButton(string actionName, string keyButton)
    {
        return ActionAddKeyButton(actionName, (Key)(long)keyButton[0]);
    }

    public static bool ActionAddMouseButton(string actionName, MouseButton buttonIndex)
    {
        if (InputMap.HasAction(actionName))
        {
            InputEventMouseButton mouseEvent = new InputEventMouseButton();
            mouseEvent.ButtonIndex = buttonIndex;
            mouseEvent.Pressed = true;
            mouseEvent.Device = -1;
            InputMap.ActionAddEvent(actionName, mouseEvent);
            return true;
        }
        return false;
    }
    public static bool ActionAddMouseButton(string actionName, double buttonIndex)
    {
        return ActionAddMouseButton(actionName, (MouseButton)buttonIndex);
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
    public static double GetActionAxis(string negativeAction, string positiveAction)
    {
        return Input.GetAxis(negativeAction, positiveAction);
    }


    // ==--==--==--==--==

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

    public Vector2 GetMousePosition()
    {
        return GetGlobalMousePosition();
    }
    public Vector2 GetViewportMousePosition()
    {
        Viewport viewport = GetViewport();
        if (viewport != null)
            return viewport.GetMousePosition();
        return GetGlobalMousePosition();
    }
    public Vector2 GetScreenMousePosition()
    {
        return (Vector2)DisplayServer.MouseGetPosition();
    }
}
