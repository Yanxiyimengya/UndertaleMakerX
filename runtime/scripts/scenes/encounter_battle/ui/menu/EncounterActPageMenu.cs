using Godot;
using System;

public partial class EncounterActPageMenu : EncounterChoicePageMenu
{
	[Export]
	BattlePlayerActMenuState battlePlayerActMenuState;
	public override void UIVisible()
	{
		ClearItem();
		BaseEnemy enemy = GlobalBattleManager.Instance.EnemysList[battlePlayerActMenuState.EnemyChoice];
		foreach (string act in enemy.Actions)
		{ 
			AddItem(act, act);
		}
	}
	public override void UIHidden()
	{

	}
}
