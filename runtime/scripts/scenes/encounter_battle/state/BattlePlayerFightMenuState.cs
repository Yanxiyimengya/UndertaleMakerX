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
    public EncounterChoiceEnemyMenu ChoiceEnemyMenu { get; set; }

    [Export]
    public EncounterAttackGaugeBarMenu GaugeBar { get; set; }
    [Export]
    public BattleScreenButtonManager BattleButtonManager { get; set; }
    #endregion

    public int EnemyChoice = 0;

    private int _state = 0;
    private float _damage = 0f;
    private bool _isTargetMiss = false;
    private BattleAttackAnimation _attackAnimation;
    private BattleDamageText _attackDamageText;

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

        if (UtmxPlayerDataManager.Weapon != null && !missed)
        {
            _damage = (float)UtmxPlayerDataManager.Weapon._CalculateDamage(hitValue, targetEnemy);
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
        BaseWeapon currentWeapon = UtmxPlayerDataManager.Weapon;
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
        EnemyChoice = Math.Clamp(EnemyChoice, 0, UtmxBattleManager.Instance.GetEnemysCount() - 1);
        return UtmxBattleManager.Instance.EnemysList[EnemyChoice];
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
            int previousChoice = EnemyChoice;
            EnemyChoice = Math.Max(EnemyChoice - 1, 0);

            if (previousChoice != EnemyChoice)
            {
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }

            ChoiceEnemyMenu?.SetChoice(EnemyChoice);
        }
        else if (Input.IsActionJustPressed("down"))
        {
            if (UtmxBattleManager.Instance.GetEnemysCount() == 0) return;

            int previousChoice = EnemyChoice;
            EnemyChoice = Math.Min(EnemyChoice + 1, UtmxBattleManager.Instance.GetEnemysCount() - 1);

            if (previousChoice != EnemyChoice)
            {
                UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SQUEAK"));
            }

            ChoiceEnemyMenu?.SetChoice(EnemyChoice);
        }
        if (Input.IsActionJustPressed("confirm"))
        {
            await OpenAttackGaugeMenu();
            _state = STATE_ATTACK_GAUGE;
            UtmxGlobalStreamPlayer.Instance.PlaySoundFromStream(UtmxGlobalStreamPlayer.Instance.GetStreamFormLibrary("SELECT"));
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
        UtmxBattleManager.Instance.GetPlayerSoul().Visible = false;
        await MenuManager.OpenMenu("EncounterAttackGaugeBarMenu");
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        BattleButtonManager.ResetAllBattleButton();
    }
    #endregion

    #region 状态机生命周期
    public override async void _EnterState()
    {
        await OpenEnemyChoiceMenu();
        EnemyChoice = Math.Clamp(EnemyChoice, 0, ChoiceEnemyMenu.GetItemCount() - 1);
        ChoiceEnemyMenu.SetChoice(EnemyChoice);
        ChoiceEnemyMenu.HpBarSetVisible(true);
        _state = STATE_SELECT_ENEMY;
    }

    private void _NextState()
    {
        SwitchState("BattlePlayerDialogState");
    }

    private async Task OpenEnemyChoiceMenu()
    {
        await MenuManager.OpenMenu("ChoiceEnemyMenu");
    }

    public override void _ExitState()
    {
        UtmxBattleManager.Instance.GetPlayerSoul().Visible = true;

    }
    public override bool _CanEnterState()
    {
        return UtmxBattleManager.Instance.GetEnemysCount() > 0;
    }
    #endregion
}
