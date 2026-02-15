using Godot;
using System;

public partial class BattleUiController : Node
{
	[Export]
	BattleScreenButtonManager ButtonManager;
	[Export]
	Control UiLayer;
	[Export]
	BattleStatusBar StatusBar;

	public bool UiVisible {
		get => UiLayer.Visible; 
		set
		{
			UiLayer.Visible = value;
		}
	}

}
