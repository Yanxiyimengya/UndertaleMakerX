using Godot;

[GlobalClass]
public partial class VirtualActionButton : TextureButton
{
	[Export] public string ActionName = "";
	[Export] public bool PressOnTouchDown = true;
	[Export] public bool KeepPressedWhileHolding = true;

	private bool _pressed;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Stop;
		ToggleMode = false;
	}

	public override void _ExitTree()
	{
		ReleaseAction();
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (string.IsNullOrEmpty(ActionName))
			return;
		
		if (@event is InputEventScreenTouch touch)
		{
			if (touch.Pressed)
			{
				if (PressOnTouchDown)
					PressAction();
			}
			else
			{
				ReleaseAction();
			}

			AcceptEvent();
			return;
		}
	}
	
	public override void _Pressed()
	{
		if (!PressOnTouchDown)
			PressAction();
	}

	public override void _Toggled(bool toggledOn)
	{
		if (toggledOn) PressAction();
		else ReleaseAction();
	}

	private void PressAction()
	{
		if (_pressed) return;

		_pressed = true;

		if (KeepPressedWhileHolding)
			Input.ActionPress(ActionName);
		else
			Input.ActionPress(ActionName, 1.0f);
	}

	private void ReleaseAction()
	{
		if (!_pressed) return;

		_pressed = false;
		Input.ActionRelease(ActionName);
	}
}
