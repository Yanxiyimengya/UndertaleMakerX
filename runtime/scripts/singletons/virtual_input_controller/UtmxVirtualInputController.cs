using Godot;
using System;

public partial class UtmxVirtualInputController : CanvasLayer
{
	public override void _Ready()
	{
		Visible = UtmxRuntimeProjectConfig.TryGetDefault<bool>("application/virtual_input", false);
	}
}
