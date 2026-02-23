using Godot;

public partial class VirtualJoystick : TextureRect
{
    public enum JoystickDirectionMode
    {
        AllDirections = 0,
        EightDirections = 1,
    }

    [ExportGroup("Nodes")]
    [Export] public NodePath StickPath;

    [ExportGroup("Joystick")]
    [Export] public float Radius = 65f;
    [Export] public float DeadZone = 0.15f;
    [Export] public float KeyThreshold = 0.1f;
    [Export] public bool VisibleOnlyWhenPressed = false;
    [Export] public JoystickDirectionMode DirectionMode = JoystickDirectionMode.AllDirections;

    [ExportGroup("Actions")]
    [Export] public string ActionLeft = "left";
    [Export] public string ActionRight = "right";
    [Export] public string ActionUp = "up";
    [Export] public string ActionDown = "down";

    public Vector2 Output { get; private set; } = Vector2.Zero;

    private Control _stick;
    private bool _dragging;
    private int _touchIndex = -1;
    private Vector2 _baseCenterGlobal;
    private Vector2 _stickOriginLocal;
    private bool _l;
    private bool _r;
    private bool _u;
    private bool _d;

    public override void _Ready()
    {
        _stick = GetNodeOrNull<Control>(StickPath);
        KeyThreshold = UtmxRuntimeProjectConfig.TryGetDefault<float>("virtual_input/dead_zone", KeyThreshold);
        ApplyDirectionModeFromConfig();

        if (_stick == null)
        {
            GD.PushError("[VirtualJoystick] StickPath not set!");
            return;
        }

        _stickOriginLocal = _stick.Position;

        if (VisibleOnlyWhenPressed)
            Visible = false;

        ResetStick();
    }

    public override void _ExitTree()
    {
        ReleaseAllActions();
    }

    public override void _Process(double delta)
    {
        _baseCenterGlobal = GlobalPosition + Size * 0.5f;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch)
        {
            if (touch.Pressed)
            {
                if (_dragging)
                    return;

                _dragging = true;
                _touchIndex = touch.Index;

                if (VisibleOnlyWhenPressed)
                    Visible = true;

                UpdateByGlobalPos(touch.Position + GlobalPosition);
                AcceptEvent();
            }
            else
            {
                if (!_dragging || touch.Index != _touchIndex)
                    return;
                OnDragFinished();
                AcceptEvent();
            }
            return;
        }

        if (@event is InputEventScreenDrag drag)
        {
            if (!_dragging || drag.Index != _touchIndex)
                return;

            UpdateByGlobalPos(drag.Position + GlobalPosition);
            AcceptEvent();
        }
    }

    private void OnDragFinished()
    {
        _dragging = false;
        _touchIndex = -1;

        ResetStick();
        ReleaseAllActions();

        if (VisibleOnlyWhenPressed)
            Visible = false;
    }

    private void UpdateByGlobalPos(Vector2 pointerGlobalPos)
    {
        _baseCenterGlobal = GlobalPosition + Size * 0.5f;

        Vector2 delta = pointerGlobalPos - _baseCenterGlobal;
        float effectiveRadius = Mathf.Max(Radius, 0.0001f);

        float len = delta.Length();
        if (len > effectiveRadius)
            delta = delta / len * effectiveRadius;

        Vector2 output = delta / effectiveRadius;
        if (output.Length() < DeadZone)
            output = Vector2.Zero;
        else
            output = ApplyDirectionMode(output);

        Output = output;

        Vector2 visualDelta = Output * effectiveRadius;
        _stick.GlobalPosition = (_baseCenterGlobal + visualDelta) - (_stick.Size * 0.5f);
        UpdateActionsFromVector(Output);
    }

    private void ApplyDirectionModeFromConfig()
    {
        float configuredMode = UtmxRuntimeProjectConfig.TryGetDefault<float>(
            "virtual_input/joystick_mode",
            (float)DirectionMode
        );
        int modeIndex = Mathf.RoundToInt(configuredMode);
        if (modeIndex == (int)JoystickDirectionMode.AllDirections ||
            modeIndex == (int)JoystickDirectionMode.EightDirections)
        {
            DirectionMode = (JoystickDirectionMode)modeIndex;
        }
    }

    private Vector2 ApplyDirectionMode(Vector2 input)
    {
        if (DirectionMode != JoystickDirectionMode.EightDirections)
            return input;
        if (input == Vector2.Zero)
            return Vector2.Zero;

        float length = input.Length();
        if (Mathf.IsZeroApprox(length))
            return Vector2.Zero;

        float step = Mathf.Pi / 4.0f;
        float snappedAngle = Mathf.Round(input.Angle() / step) * step;
        return Vector2.FromAngle(snappedAngle) * length;
    }

    private void UpdateActionsFromVector(Vector2 v)
    {
        bool left = v.X <= -KeyThreshold;
        bool right = v.X >= KeyThreshold;
        bool up = v.Y <= -KeyThreshold;
        bool down = v.Y >= KeyThreshold;

        SetAction(ActionLeft, left, ref _l);
        SetAction(ActionRight, right, ref _r);
        SetAction(ActionUp, up, ref _u);
        SetAction(ActionDown, down, ref _d);
    }

    private void SetAction(string action, bool pressed, ref bool prev)
    {
        if (string.IsNullOrEmpty(action))
            return;

        if (pressed == prev)
            return;
        prev = pressed;

        if (pressed)
            Input.ActionPress(action);
        else
            Input.ActionRelease(action);
    }

    private void ResetStick()
    {
        Output = Vector2.Zero;
        _stick.Position = _stickOriginLocal;
    }

    private void ReleaseAllActions()
    {
        if (!string.IsNullOrEmpty(ActionLeft))
            Input.ActionRelease(ActionLeft);
        if (!string.IsNullOrEmpty(ActionRight))
            Input.ActionRelease(ActionRight);
        if (!string.IsNullOrEmpty(ActionUp))
            Input.ActionRelease(ActionUp);
        if (!string.IsNullOrEmpty(ActionDown))
            Input.ActionRelease(ActionDown);

        _l = false;
        _r = false;
        _u = false;
        _d = false;
    }
}
