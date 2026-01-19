using Godot;
using System;

public partial class EncounterActPageMenu : EncounterChoicePageMenu
{
	[Export]
	BattlePlayerActMenuState battlePlayerActMenuState;
	public override void UIVisible()
	{
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			ClearItem();
			BaseEnemy enemy = enc.Enemys[battlePlayerActMenuState.EnemyChoice];
			foreach (string act in enemy.Actions)
			{ 
				AddItem(act, act);
			}
		}
	}
	public override void UIHidden()
	{

	}
}
