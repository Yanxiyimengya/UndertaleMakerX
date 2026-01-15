using Godot;
using System;

[GlobalClass]
public partial class BattlePlayerTurnState : StateNode
{
	[Export]
	AudioStream SndSelect;
	[Export]
	AudioStream SndSqueak;
	
	[Export]
	BattlePlayerSoul battlePlayerSoul;
	[Export]
	BattleScreenButtonManager battleButtonManager;
	[Export]
	TextTyper encounterTextTyper;

	private int actionSelect = -1;

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left"))
		{
			actionSelect -= 1;
			GlobalSoundPlayer.Instance.PlaySound(SndSqueak);
			if (actionSelect < 0) actionSelect = battleButtonManager.GetButtonCount()-1;
		}
		else if (Input.IsActionJustPressed("right"))
		{
			actionSelect += 1;
			GlobalSoundPlayer.Instance.PlaySound(SndSqueak);
			if (actionSelect >= battleButtonManager.GetButtonCount()) actionSelect = 0;
		}
		SetSelect(actionSelect);

		if (battlePlayerSoul != null && battleButtonManager.GetButton(actionSelect) is BattleScreenButton btn)
		{
			battlePlayerSoul.GlobalPosition = btn.GetSoulPosition();
		}
	}

	public void SetSelect(int select)
	{
		battleButtonManager.PressButton(actionSelect);
	}

	public override void _EnterState()
	{
		actionSelect = 0;
		SetSelect(actionSelect);
		battlePlayerSoul.Movable = false;
		battlePlayerSoul.EnableCollision = false;
		battlePlayerSoul.Show();

		encounterTextTyper.Restart("* UndertaleMakerX by Yanxiyimtng.\n* Having fun! :)"); // TODO

	}

	public override void _ExitState()
	{
		battleButtonManager.ReleaseButton(actionSelect);
	}
}
