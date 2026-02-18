using Godot;
using System;

public partial class UtmxVirtualInputController : CanvasLayer
{
	public override void _Ready()
	{
		Visible = UtmxRuntimeProjectConfig.TryGetDefault<bool>("virtual_input/enabled", false);
	}
}
