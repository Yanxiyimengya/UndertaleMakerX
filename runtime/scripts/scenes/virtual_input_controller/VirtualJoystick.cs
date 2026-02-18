using Godot;
using System;

public partial class VirtualJoystick : TextureRect
{
	[ExportGroup("Nodes")]
	[Export] public NodePath StickPath;

	[ExportGroup("Joystick")]
	[Export] public float Radius = 80f;         // 摇杆最大位移像素
	[Export] public float DeadZone = 0.15f;     // 0..1
	[Export] public float KeyThreshold = 0.1f; // 0..1
	[Export] public bool VisibleOnlyWhenPressed = false;

	[ExportGroup("Actions")]
	[Export] public string ActionLeft = "left";
	[Export] public string ActionRight = "right";
	[Export] public string ActionUp = "up";
	[Export] public string ActionDown = "down";

	public Vector2 Output { get; private set; } = Vector2.Zero; // -1..1

	private Control _stick;

	private bool _dragging;
	private int _touchIndex = -1;

	private Vector2 _baseCenterGlobal;     // 自身中心点（全局坐标）
	private Vector2 _stickOriginLocal;     // Stick初始位置（本地坐标）

	private bool _l, _r, _u, _d;

	public override void _Ready()
	{
		_stick = GetNodeOrNull<Control>(StickPath);
		KeyThreshold = UtmxRuntimeProjectConfig.TryGetDefault<float>("virtual_input/dead_zone", KeyThreshold);
		
		if (_stick == null)
		{
			GD.PushError("[VirtualJoystick] StickPath not set!");
			return;
		}

		// 记录Stick相对于自身的初始位置
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
		// 每一帧更新自身中心点，以应对 UI 布局变化或移动
		_baseCenterGlobal = GlobalPosition + Size * 0.5f;
	}

	public override void _GuiInput(InputEvent @event)
	{
		// 触摸与点击
		if (@event is InputEventScreenTouch touch)
		{
			if (touch.Pressed)
			{
				if (_dragging) return;

				_dragging = true;
				_touchIndex = touch.Index;

				if (VisibleOnlyWhenPressed)
					Visible = true;

				UpdateByGlobalPos(touch.Position + GlobalPosition);
				AcceptEvent();
			}
			else
			{
				if (!_dragging || touch.Index != _touchIndex) return;
				OnDragFinished();
				AcceptEvent();
			}
			return;
		}
		
		// 拖拽
		if (@event is InputEventScreenDrag drag)
		{
			if (!_dragging || drag.Index != _touchIndex) return;

			UpdateByGlobalPos(drag.Position + GlobalPosition);
			AcceptEvent();
			return;
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
		// 1. 获取基座当前的全局中心点（每一帧都重新计算，防止 UI 缩放或移动导致偏移）
		_baseCenterGlobal = GlobalPosition + Size * 0.5f;

		// 2. 计算手指距离基座中心的偏移向量（全局）
		Vector2 delta = pointerGlobalPos - _baseCenterGlobal;

		// 3. 限制半径：如果手指超出 Radius，将其约束在圆周上
		float len = delta.Length();
		if (len > Radius)
			delta = delta / len * Radius;

		// 4. 设置 Stick 的位置（这是最容易偏的一步）
		// 正确逻辑：Stick全局左上角 = 基座全局中心 + 偏移向量 - Stick自己大小的一半
		_stick.GlobalPosition = (_baseCenterGlobal + delta) - (_stick.Size * 0.5f);

		// 5. 计算逻辑输出 (-1 到 1)
		Vector2 v = delta / Radius;
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
		// 回到初始本地位置
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
