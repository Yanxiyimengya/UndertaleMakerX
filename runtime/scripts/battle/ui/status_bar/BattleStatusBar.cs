using Godot;
using System;
using System.Text.RegularExpressions;

[Tool]
public partial class BattleStatusBar : Control
{
	[Export]
	public string PlayerName {
		get => playerName;
		set 
		{
			playerName = value;
			updateNameLvLabel();
		}
	}

	[Export]
	public int PlayerLv
	{
		get => playerLv;
		set
		{
			playerLv = value;
			updateNameLvLabel();
		}
	}

	[Export]
	public double PlayerHp
	{
		get => playerHp;
		set
		{
			playerHp = value;
			updateHp();
		}
	}
	[Export]
	public double PlayerMaxHp
	{
		get => playerMaxHp;
		set
		{
			playerMaxHp = value;
			updateHp();
		}
	}


	[Export]
	public Label NameLvLabel;
	[Export]
	public ProgressBar HpProgressBarLabel;
	[Export]
	public Label HpLabel;

	private string playerName;
	private int playerLv;
	private double playerHp = 20.0;
	private double playerMaxHp = 20.0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		updateNameLvLabel();
		updateHp();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void updateNameLvLabel()
	{
		if (NameLvLabel != null)
		{
			NameLvLabel.Text = $"{playerName}   LV {playerLv}";
		}
	}
	private void updateHp()
	{
		if (HpProgressBarLabel != null)
		{
			HpProgressBarLabel.MaxValue = playerMaxHp;
			HpProgressBarLabel.Value = playerHp;
			HpProgressBarLabel.CustomMinimumSize = new Vector2(
				(float)Math.Round(playerMaxHp * 1.2F) + 1.0F, 21.0F);
		}
	}
}
