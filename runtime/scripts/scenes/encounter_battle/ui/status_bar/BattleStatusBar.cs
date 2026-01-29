using Godot;

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
    public StatueBarHpProgressBar HpProgressBar;
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
        _playerName = UtmxPlayerDataManager.PlayerName;
        _playerLv = UtmxPlayerDataManager.PlayerLv;
        _playerMaxHp = UtmxPlayerDataManager.PlayerMaxHp;
        _playerHp = UtmxPlayerDataManager.PlayerHp;
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
        if (HpProgressBar != null)
        {
            HpProgressBar.MaxValue = _playerMaxHp;
            HpProgressBar.Value = _playerHp;
            HpLabel.Text = $"{_playerHp} / {_playerMaxHp}";
        }
    }
}
