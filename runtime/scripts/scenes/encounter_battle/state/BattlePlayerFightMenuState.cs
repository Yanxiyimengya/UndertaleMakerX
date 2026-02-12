using Godot;
using System;
using System.Threading.Tasks;

// 全局类标记保持不变
[GlobalClass]
public partial class BattlePlayerFightMenuState : StateNode
{
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

	public int EnemyChoice = 0;

	private int _state = 0;
	private float _damage = 0f;
	private bool _isTargetMiss = false;
	private BattleAttackAnimation _attackAnimation;
	private BattleDamageText _attackDamageText;

	private const int STATE_SELECT_ENEMY = 0;
	private const int STATE_ATTACK_GAUGE = 1;
	private const int STATE_SHOW_DAMAGE = 2;

	public override void _Ready()
	{
		GaugeBar?.Connect(
			EncounterAttackGaugeBarMenu.SignalName.Hitted,
			Callable.From((bool missed, float hitValue) => OnGaugeBarHitted(missed, hitValue)));
	}

	private void OnGaugeBarHitted(bool missed, float hitValue)
	{
		_isTargetMiss = missed;
		_attackAnimation = null;

		BaseEnemy targetEnemy = GetTargetEnemy();
		if (targetEnemy == null) return;

		if (UtmxPlayerDataManager.Weapon != null && !missed)
		{
			_damage = (float)UtmxPlayerDataManager.Weapon.onAttack(hitValue, targetEnemy);
			SpawnAttackAnimation(targetEnemy);
		}
		if (_attackAnimation == null)
		{
			ShowDamageText(targetEnemy);
		}
		targetEnemy._HandleAttack(missed ? UtmxBattleManager.AttackStatus.Missed : UtmxBattleManager.AttackStatus.Hit);
	}

	private void ShowDamageText(BaseEnemy targetEnemy)
	{
		_attackDamageText = (BattleDamageText)DamageTextPackedScene?.Instantiate();
		UtmxBattleManager.GetBattleEnemyController().EnemiesNode.AddChild(_attackDamageText);
		_attackDamageText.Start(targetEnemy.Position + targetEnemy.CenterPosition);
		targetEnemy.hurt(_damage);

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
		_attackAnimation = new BattleAttackAnimationFrame();
		_attackAnimation.TexturesPath = currentWeapon.AttackAnimation;
		_attackAnimation.Play();
		UtmxBattleManager.GetBattleEnemyController().EnemiesNode.AddChild(_attackAnimation);
		_attackAnimation.GlobalPosition = targetEnemy.GlobalPosition + targetEnemy.CenterPosition;

		_attackAnimation.Connect(
			BattleAttackAnimation.SignalName.Finished,
			Callable.From(() => ShowDamageText(targetEnemy)));
	}

	private BaseEnemy GetTargetEnemy()
	{
		int enemysCount = UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount();
		EnemyChoice = Math.Clamp(EnemyChoice, 0, enemysCount - 1);
		return UtmxBattleManager.GetBattleEnemyController().GetEnemy(EnemyChoice);
	}

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
				UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
			}

			ChoiceEnemyMenu?.SetChoice(EnemyChoice);
		}
		else if (Input.IsActionJustPressed("down"))
		{
			int enemysCount = UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount();
			if (enemysCount < 1) return;
			
			int previousChoice = EnemyChoice;
			EnemyChoice = Math.Min(EnemyChoice + 1, enemysCount - 1);
			
			if (previousChoice != EnemyChoice)
			{
				UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SQUEAK"));
			}

			ChoiceEnemyMenu?.SetChoice(EnemyChoice);
		}
		if (Input.IsActionJustPressed("confirm"))
		{
			await OpenAttackGaugeMenu();
			_state = STATE_ATTACK_GAUGE;
			UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("SELECT"));
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

	private async Task _OpenEnemyChoiceMenu()
	{
		ChoiceEnemyMenu.HpBarSetVisible(true);
		await MenuManager.OpenMenu("EncounterChoiceEnemyMenu");
		EnemyChoice = Math.Clamp(EnemyChoice, 0, 
			UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount());
		ChoiceEnemyMenu.SetChoice(EnemyChoice);
		_state = STATE_SELECT_ENEMY;
	}

	private async Task OpenAttackGaugeMenu()
	{
		UtmxBattleManager.GetBattlePlayerController().PlayerSoul.Visible = false;
		await MenuManager.OpenMenu("EncounterAttackGaugeBarMenu");
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		BattleButtonManager.ResetAllBattleButton();
	}

	public override async void _EnterState()
	{
		await _OpenEnemyChoiceMenu();
	}

	private void _NextState()
	{
		UtmxBattleManager.GetBattleController().ChangeToPlayerDialogueState();
	}

	public override void _ExitState()
	{
		UtmxBattleManager.GetBattlePlayerController().PlayerSoul.Visible = true;

	}
	public override bool _CanEnterState()
	{
		int enemysCount = UtmxBattleManager.GetBattleEnemyController().GetEnemiesCount();
		return enemysCount > 0;
	}
}
