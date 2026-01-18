using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class BattlePlayerFightMenuState : StateNode
{
	[Export]
	public AudioStream SndSelect;
	[Export]
	public AudioStream SndSqueak;

	[Export]
	public BattleMenuManager MenuManager;
	[Export]
	public EncounterFightChoiceEnemyMenu encounterFightChoiceEnemyMenu;
	
	public int EnemyChoice = 0;

	private EncounterBattle _encounterBattle;

	public override void _Ready()
	{
		_encounterBattle = GetTree().CurrentScene as EncounterBattle;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("up"))
		{
			int newChoice = EnemyChoice - 1;
			if (newChoice != EnemyChoice)
			{
				EnemyChoice = Math.Max(0, newChoice);
				GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
				encounterFightChoiceEnemyMenu.SetChoice(EnemyChoice);
			}
		}
		else if (Input.IsActionJustPressed("down"))
		{
			if (_encounterBattle != null && _encounterBattle.Enemys.Count > 0)
			{
				int newChoice = EnemyChoice + 1;
				if (newChoice != EnemyChoice)
				{
					EnemyChoice = Math.Min(newChoice, _encounterBattle.Enemys.Count - 1);
					GlobalStreamPlayer.Instance.PlaySound(SndSqueak);
					encounterFightChoiceEnemyMenu.SetChoice(EnemyChoice);
				}
			}
		}

		if (Input.IsActionJustPressed("cancel"))
		{
			EmitSignal(SignalName.RequestSwitchState, ["BattlePlayerChoiceActionState"]);
		}
	}

	public override async void _EnterState()
	{
		await MenuManager.OpenMenu("EncounterFightChoiceEnemyMenu");
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		encounterFightChoiceEnemyMenu.SetChoice(EnemyChoice);
	}

	public override void _ExitState()
	{
	}
}
