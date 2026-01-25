using Godot;
using System;
using System.Text.RegularExpressions;

[GlobalClass]
public partial class BattleStatusBar : Control
{
	[Export]
	public string PlayerName;
	[Export]
	public int PlayerLv;
	[Export]
	public double PlayerHp;
	[Export]
	public double PlayerMaxHp;


	[Export]
	public Label NameLvLabel;
	[Export]
	public ProgressBar HpProgressBarLabel;
	[Export]
	public Label HpLabel;

	private string _playerName;
	private int _playerLv;
	private double _playerHp = 20.0;
	private double _playerMaxHp = 20.0;

	public override void _Ready()
	{
		UpdatePlayerData();
	}

	public override void _Process(double delta)
	{
		UpdatePlayerData();
	}

	public void UpdatePlayerData()
	{
		_playerName = PlayerDataManager.Instance.PlayerName;
		_playerLv = PlayerDataManager.Instance.PlayerLv;
		_playerMaxHp = PlayerDataManager.Instance.PlayerMaxHp;
		_playerHp = PlayerDataManager.Instance.PlayerHp;
		updateNameLvLabel();
		updateHp();
	}

	private void updateNameLvLabel()
	{
		if (NameLvLabel != null)
		{
			NameLvLabel.Text = $"{_playerName}   LV {_playerLv}";
		}
	}
	private void updateHp()
	{
		if (HpProgressBarLabel != null)
		{
			HpProgressBarLabel.MaxValue = _playerMaxHp;
			HpProgressBarLabel.Value = _playerHp;
			HpProgressBarLabel.CustomMinimumSize = new Vector2(
				(float)Math.Round(_playerMaxHp * 1.2F) + 1.0F, 21.0F);
			HpLabel.Text = $"{_playerHp} / {_playerMaxHp}";

		}
	}
}
