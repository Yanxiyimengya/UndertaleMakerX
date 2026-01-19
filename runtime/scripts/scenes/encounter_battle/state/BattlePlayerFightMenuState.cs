using Godot;
using System;
using System.Threading.Tasks;

// 全局类标记保持不变
[GlobalClass]
public partial class BattlePlayerFightMenuState : StateNode
{
	#region 导出配置（序列化字段）
	[Export]
	public AudioStream SndSelect { get; set; } // 改为属性，提升封装性

	[Export]
	public AudioStream SndSqueak { get; set; }

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
	private EncounterBattle _encounterBattle; // 当前战斗场景引用
	private int _state = 0; // 状态机：0-选择敌人 1-攻击计量条 2-显示伤害文本
	private float _damage = 0f; // 计算出的伤害值
	private bool _isMiss = false; // 是否未命中（重命名：避免关键字冲突）
	private BattleAttackAnimation _attackAnimation; // 攻击动画实例
	private BattleDamageText _attackDamageText; // 伤害文本实例

	private const int STATE_SELECT_ENEMY = 0;
	private const int STATE_ATTACK_GAUGE = 1;
	private const int STATE_SHOW_DAMAGE = 2;

	#region 生命周期方法
	public override void _Ready()
	{
		_encounterBattle = GetTree().CurrentScene as EncounterBattle;
		GaugeBar?.Connect(
			EncounterAttackGaugeBarMenu.SignalName.Hitted,
			Callable.From((bool missed, float hitValue) => OnGaugeBarHitted(missed, hitValue)));
	}
	#endregion

	#region 核心业务逻辑
	private void OnGaugeBarHitted(bool missed, float hitValue)
	{
		_isMiss = missed;
		_attackAnimation = null;

		BaseEnemy targetEnemy = GetTargetEnemy();
		if (targetEnemy == null) return;

		if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.Weapon != null)
		{
			_damage = (float)PlayerDataManager.Instance.Weapon._CalculateDamage(hitValue, targetEnemy);
			_isMiss = _isMiss || _damage <= 0;
			SpawnAttackAnimation(targetEnemy);
		}

		if (_attackAnimation == null)
		{
			ShowDamageText(targetEnemy);
		}

		targetEnemy.HandleAttack(_isMiss);
	}

	private void ShowDamageText(BaseEnemy targetEnemy)
	{
		_attackDamageText = (BattleDamageText)DamageTextPackedScene?.Instantiate();
		targetEnemy.AddChild(_attackDamageText);
		_state = STATE_SHOW_DAMAGE;

		if (_isMiss)
		{
			_attackDamageText.SetText(targetEnemy.MissText);
		}
		else
		{
			_attackDamageText.SetNumber((int)_damage);
		}
		_attackDamageText.Connect(
			BattleDamageText.SignalName.Ended,
			Callable.From(async void () => {
				await GaugeBar.End();
				 _NextState();
				}),
			(uint)Node.ConnectFlags.OneShot);
	}

	private void SpawnAttackAnimation(BaseEnemy targetEnemy)
	{
		BaseWeapon currentWeapon = PlayerDataManager.Instance.Weapon;
		if (currentWeapon?.AttackAnimation == null) return;

		_attackAnimation = (BattleAttackAnimation)currentWeapon.AttackAnimation?.Instantiate();
		_attackAnimation.GlobalPosition = targetEnemy.GlobalPosition;
		AddChild(_attackAnimation);

		_attackAnimation.Connect(
			BattleAttackAnimation.SignalName.Finished,
			Callable.From(() => ShowDamageText(targetEnemy)));
	}

	private BaseEnemy GetTargetEnemy()
	{
		_enemyChoice = Math.Clamp(_enemyChoice, 0, _encounterBattle.Enemys.Count - 1);
		return _encounterBattle.Enemys[_enemyChoice];
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

			if (previousChoice != _enemyChoice && SndSqueak != null)
			{
				GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			}

			EncounterChoiceEnemyMenu?.SetChoice(_enemyChoice);
		}
		else if (Input.IsActionJustPressed("down"))
		{
			if (_encounterBattle?.Enemys == null || _encounterBattle.Enemys.Count == 0) return;

			int previousChoice = _enemyChoice;
			_enemyChoice = Math.Min(_enemyChoice + 1, _encounterBattle.Enemys.Count - 1);

			if (previousChoice != _enemyChoice && SndSqueak != null)
			{
				GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
			}

			EncounterChoiceEnemyMenu?.SetChoice(_enemyChoice);
		}
		if (Input.IsActionJustPressed("confirm"))
		{
			await OpenAttackGaugeMenu();
			_state = STATE_ATTACK_GAUGE;
			GlobalStreamPlayer.Instance.PlaySound(SndSelect);
		}
		else if (Input.IsActionJustPressed("cancel"))
		{
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
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
		if (_encounterBattle?.GetPlayerSoul() is BattlePlayerSoul soul)
		{
			soul.Visible = false;
		}
		await MenuManager.OpenMenu("EncounterAttackGaugeBarMenu");
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		BattleButtonManager.ReleaseAllButton();
	}
	#endregion

	#region 状态机生命周期
	public override async void _EnterState()
	{
		EncounterChoiceEnemyMenu.HpBarSetVisible(true);
		await OpenEnemyChoiceMenu();
		EncounterChoiceEnemyMenu.SetChoice(_enemyChoice);
		_state = STATE_SELECT_ENEMY;
	}

	private void _NextState()
	{
		EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerDialogState"]);
	}

	private async Task OpenEnemyChoiceMenu()
	{
		await MenuManager.OpenMenu("EncounterChoiceEnemyMenu");
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
	}

	public override void _ExitState() {
		if (_encounterBattle?.GetPlayerSoul() is BattlePlayerSoul soul)
		{
			soul.Visible = true;
		}
	}
	#endregion
}
