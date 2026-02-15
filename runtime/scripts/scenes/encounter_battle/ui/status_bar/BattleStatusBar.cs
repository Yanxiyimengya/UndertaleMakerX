using Godot;
using System.Text;

[GlobalClass]
public partial class BattleStatusBar : Control
{
    [Export]
    public Label NameLvLabel;
    [Export]
    public StatueBarHpProgressBar HpProgressBar;
    [Export]
    public Label HpLabel;

    private readonly StringBuilder _nameLvSb = new StringBuilder(32);
    private readonly StringBuilder _hpSb = new StringBuilder(32);

    private string _lastPlayerName = string.Empty;
    private int _lastPlayerLv;
    private double _lastPlayerHp = -1;
    private double _lastPlayerMaxHp = -1;
    public override void _Ready()
    {
        UpdatePlayerData();
    }

    public override void _Process(double delta)
    {
        if (IsVisibleInTree())
        {
            UpdatePlayerData();
        }
    }

    private void UpdatePlayerData()
    {
        string currentName = UtmxPlayerDataManager.PlayerName;
        int currentLv = (int)UtmxPlayerDataManager.PlayerLv;
        double currentHp = UtmxPlayerDataManager.PlayerHp;
        double currentMaxHp = UtmxPlayerDataManager.PlayerMaxHp;

        bool isNameLvChanged = currentName != _lastPlayerName || currentLv != _lastPlayerLv;
        bool isHpChanged = currentHp != _lastPlayerHp || currentMaxHp != _lastPlayerMaxHp;

        if (isNameLvChanged && NameLvLabel != null)
        {
            _lastPlayerName = currentName;
            _lastPlayerLv = currentLv;

            _nameLvSb.Clear();
            _nameLvSb.Append(currentName)
                     .Append("   LV ")
                     .Append(currentLv);

            if (NameLvLabel.Text != _nameLvSb.ToString())
            {
                NameLvLabel.Text = _nameLvSb.ToString();
            }
        }

        if (isHpChanged && HpProgressBar != null && HpLabel != null)
        {
            _lastPlayerHp = currentHp;
            _lastPlayerMaxHp = currentMaxHp;

            HpProgressBar.MaxValue = currentMaxHp;
            HpProgressBar.Value = currentHp;

            _hpSb.Clear();
            _hpSb.Append(currentHp)
                 .Append(" / ")
                 .Append(currentMaxHp);

            if (HpLabel.Text != _hpSb.ToString())
            {
                HpLabel.Text = _hpSb.ToString();
            }
        }
    }
}