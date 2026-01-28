using Godot;
using System;
using System.Threading.Tasks;

// 全局类标记保持不变
[GlobalClass]
public partial class BattlePlayerFightMenuState : StateNode
{
    #region 导出配置（序列化字段）
    [Export]
    public PackedScene DamageTextPackedScene { get; set; }

    [Export]
    public BattleMenuManager MenuManager { get; set; }

    [Export]
    public EncounterChoiceEnemyMenu EncounterChoiceEnemyMenu { get; set; } // 命名规范：帕斯卡命名法

    [Export]
    public EncounterAttackGaugeBarMenu GaugeBar { get; set; }
    [Export]
    public BattleScreenButtonManager BattleButtonManager { get; set; }
    #endregion

    private int _enemyChoice = 0;
    private int _state = 0; // 状态机：0-选择敌人 1-攻击计量条 2-显示伤害文本
    private float _damage = 0f; // 计算出的伤害
    private bool _isTargetMiss = false; // 是否未命中
    private BattleAttackAnimation _attackAnimation; // 攻击动画实例
    private BattleDamageText _attackDamageText; // 伤害文本实例

    private const int STATE_SELECT_ENEMY = 0;
    private const int STATE_ATTACK_GAUGE = 1;
    private const int STATE_SHOW_DAMAGE = 2;

    #region 生命周期方法
    public override void _Ready()
    {
        GaugeBar?.Connect(
            EncounterAttackGaugeBarMenu.SignalName.Hitted,
            Callable.From((bool missed, float hitValue) => OnGaugeBarHitted(missed, hitValue)));
    }
    #endregion

    #region 核心业务逻辑
    private void OnGaugeBarHitted(bool missed, float hitValue)
    {
        _isTargetMiss = missed;
        _attackAnimation = null;

        BaseEnemy targetEnemy = GetTargetEnemy();
        if (targetEnemy == null) return;

        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.Weapon != null && !missed)
        {
            _damage = (float)PlayerDataManager.Instance.Weapon._CalculateDamage(hitValue, targetEnemy);
            SpawnAttackAnimation(targetEnemy);
        }

        if (_attackAnimation == null)
        {
            ShowDamageText(targetEnemy);
        }

        targetEnemy.HandleAttack(_isTargetMiss);
    }

    private void ShowDamageText(BaseEnemy targetEnemy)
    {
        _attackDamageText = (BattleDamageText)DamageTextPackedScene?.Instantiate();
        _attackDamageText.Position = targetEnemy.CenterPosition;
        targetEnemy.AddChild(_attackDamageText);
        _state = STATE_SHOW_DAMAGE;

        _attackDamageText.Connect(
            BattleDamageText.SignalName.Ended,
            Callable.From(async void () =>
            {
                await GaugeBar.End();
                _NextState();
            }),
            (uint)Node.ConnectFlags.OneShot);
        if (_isTargetMiss)
        {
            _attackDamageText.SetText(targetEnemy.MissText);
            _attackDamageText.End();
        }
        else
        {
            _attackDamageText.SetNumber((int)_damage);
        }
    }

    private void SpawnAttackAnimation(BaseEnemy targetEnemy)
    {
        BaseWeapon currentWeapon = PlayerDataManager.Instance.Weapon;
        if (currentWeapon?.AttackAnimation == null) return;

        _attackAnimation = (BattleAttackAnimation)currentWeapon.AttackAnimation?.Instantiate();
        _attackAnimation.GlobalPosition = targetEnemy.GlobalPosition + targetEnemy.CenterPosition;
        AddChild(_attackAnimation);

        _attackAnimation.Connect(
            BattleAttackAnimation.SignalName.Finished,
            Callable.From(() => ShowDamageText(targetEnemy)));
    }

    private BaseEnemy GetTargetEnemy()
    {
        _enemyChoice = Math.Clamp(_enemyChoice, 0, GlobalBattleManager.Instance.GetEnemysCount() - 1);
        return GlobalBattleManager.Instance.EnemysList[_enemyChoice];
    }
    #endregion

    #region 输入处理（状态机）
    public override void _Process(double delta)
    {
        switch (_state)
        {
            case STATE_SELECT_ENEMY:
                HandleEnemySelectionInput();
                break;
            case STATE_ATTACK_GAUGE:
                HandleAttackGaugeInput();
                break;
            case STATE_SHOW_DAMAGE:
                break;
        }
    }

    private async void HandleEnemySelectionInput()
    {
        if (Input.IsActionJustPressed("up"))
        {
            int previousChoice = _enemyChoice;
            _enemyChoice = Math.Max(_enemyChoice - 1, 0);

            if (previousChoice != _enemyChoice)
            {
                UtmxGlobalStreamPlayer.Instance.PlaySound(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }

            EncounterChoiceEnemyMenu?.SetChoice(_enemyChoice);
        }
        else if (Input.IsActionJustPressed("down"))
        {
            if (GlobalBattleManager.Instance.GetEnemysCount() == 0) return;

            int previousChoice = _enemyChoice;
            _enemyChoice = Math.Min(_enemyChoice + 1, GlobalBattleManager.Instance.GetEnemysCount() - 1);

            if (previousChoice != _enemyChoice)
            {
                UtmxGlobalStreamPlayer.Instance.PlaySound(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }

            EncounterChoiceEnemyMenu?.SetChoice(_enemyChoice);
        }
        if (Input.IsActionJustPressed("confirm"))
        {
            await OpenAttackGaugeMenu();
            _state = STATE_ATTACK_GAUGE;
            UtmxGlobalStreamPlayer.Instance.PlaySound(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SELECT"));
        }
        else if (Input.IsActionJustPressed("cancel"))
        {
            SwitchState("BattlePlayerChoiceActionState");
        }
    }

    private void HandleAttackGaugeInput()
    {
        if (Input.IsActionJustPressed("confirm"))
        {
            GaugeBar?.Hit();
        }
    }

    private async Task OpenAttackGaugeMenu()
    {
        GlobalBattleManager.Instance.GetPlayerSoul().Visible = false;
        await MenuManager.OpenMenu("EncounterAttackGaugeBarMenu");
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        BattleButtonManager.ResetAllBattleButton();
    }
    #endregion

    #region 状态机生命周期
    public override async void _EnterState()
    {
        await OpenEnemyChoiceMenu();
        EncounterChoiceEnemyMenu.SetChoice(_enemyChoice);
        EncounterChoiceEnemyMenu.HpBarSetVisible(true);
        _state = STATE_SELECT_ENEMY;
    }

    private void _NextState()
    {
        SwitchState("BattlePlayerDialogState");
    }

    private async Task OpenEnemyChoiceMenu()
    {
        await MenuManager.OpenMenu("EncounterChoiceEnemyMenu");
    }

    public override void _ExitState()
    {
        GlobalBattleManager.Instance.GetPlayerSoul().Visible = true;

    }
    public override bool _CanEnterState()
    {
        return GlobalBattleManager.Instance.GetEnemysCount() > 0;
    }
    #endregion
}
