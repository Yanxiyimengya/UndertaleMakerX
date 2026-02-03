using Godot;
using System;

public partial class VirtualJoystick : Control
{
	[ExportGroup("Nodes")]
	[Export] public NodePath BasePath;
	[Export] public NodePath StickPath;

	[ExportGroup("Joystick")]
	[Export] public float Radius = 80f;         // 摇杆最大位移像素
	[Export] public float DeadZone = 0.15f;     // 0..1
	[Export] public float KeyThreshold = 0.45f; // 0..1
	[Export] public bool VisibleOnlyWhenPressed = false;

	[ExportGroup("Actions")]
	[Export] public string ActionLeft = "left";
	[Export] public string ActionRight = "right";
	[Export] public string ActionUp = "up";
	[Export] public string ActionDown = "down";

	public Vector2 Output { get; private set; } = Vector2.Zero; // -1..1

	private Control _base;
	private Control _stick;

	private bool _dragging;
	private int _touchIndex = -1;

	private Vector2 _baseCenterGlobal;     // Base中心点（全局坐标）
	private Vector2 _stickOriginLocal;     // Stick初始位置（本地坐标）

	private bool _l, _r, _u, _d;

	public override void _Ready()
	{
		_base = GetNodeOrNull<Control>(BasePath);
		_stick = GetNodeOrNull<Control>(StickPath);

		if (_base == null || _stick == null)
		{
			GD.PushError("[VirtualJoystick] BasePath/StickPath not set!");
			return;
		}

		_stickOriginLocal = _stick.Position;

		if (VisibleOnlyWhenPressed)
			Visible = false;

		// 确保能接收到GUI输入
		MouseFilter = MouseFilterEnum.Stop;

		ResetStick();
	}

	public override void _ExitTree()
	{
		ReleaseAllActions();
	}

	public override void _Process(double delta)
	{
		_baseCenterGlobal = _base.GlobalPosition + _base.Size * 0.5f;
	}

	public override void _GuiInput(InputEvent @event)
	{
		// 触摸
		if (@event is InputEventScreenTouch touch)
		{
			if (touch.Pressed)
			{
				if (_dragging) return;

				// 只允许在JoystickRoot区域内按下开始拖动
				_dragging = true;
				_touchIndex = touch.Index;

				if (VisibleOnlyWhenPressed)
					Visible = true;

				UpdateByGlobalPos(touch.Position);
				AcceptEvent();
			}
			else
			{
				if (!_dragging || touch.Index != _touchIndex) return;

				_dragging = false;
				_touchIndex = -1;

				ResetStick();
				ReleaseAllActions();

				if (VisibleOnlyWhenPressed)
					Visible = false;

				AcceptEvent();
			}
			return;
		}
		
		if (@event is InputEventScreenDrag drag)
		{
			if (!_dragging || drag.Index != _touchIndex) return;

			UpdateByGlobalPos(drag.Position);
			AcceptEvent();
			return;
		}
		
		if (@event is InputEventMouseMotion mm)
		{
			if (!_dragging || _touchIndex != -999) return;

			UpdateByGlobalPos(mm.Position);
			AcceptEvent();
			return;
		}
	}

	private void UpdateByGlobalPos(Vector2 pointerGlobalPos)
	{
		// 计算摇杆方向
		Vector2 delta = pointerGlobalPos - _baseCenterGlobal;

		// 限制最大半径
		float len = delta.Length();
		if (len > Radius)
			delta = delta / len * Radius;

		// 更新Stick位置（本地坐标移动）
		_stick.GlobalPosition = (_baseCenterGlobal - _stick.Size * 0.5f) + delta;

		// 输出向量归一化 -1..1
		Vector2 v = delta / Radius;

		// 死区
		if (v.Length() < DeadZone)
			v = Vector2.Zero;

		Output = v;

		UpdateActionsFromVector(Output);
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
		if (string.IsNullOrEmpty(action)) return;

		if (pressed == prev) return;
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
		if (!string.IsNullOrEmpty(ActionLeft)) Input.ActionRelease(ActionLeft);
		if (!string.IsNullOrEmpty(ActionRight)) Input.ActionRelease(ActionRight);
		if (!string.IsNullOrEmpty(ActionUp)) Input.ActionRelease(ActionUp);
		if (!string.IsNullOrEmpty(ActionDown)) Input.ActionRelease(ActionDown);

		_l = _r = _u = _d = false;
	}
}
