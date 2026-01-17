using Godot;
using System;
using System.Threading.Tasks;

public partial class EncounterActChoiceEnemyMenu : BaseEncounterMenu
{
	[Export]
	public UndertaleStyleScrollBar UtScrollBar;
	[Export]
	public Godot.Collections.Array<EncounterChoiceMenuItem> EnemyMenuItem = [null, null, null];

	private int _firstIndex = 0; // 菜单滑动窗口

	public override void UIVisible()
	{
		if (GetTree().CurrentScene is EncounterBattle enc)
		{
			UtScrollBar.Count = enc.Enemys.Count;
		}
	}
	public override void UIHidden()
	{
		
	}

	public void EnemyHpBarVisible(bool v)
	{
		foreach (EncounterChoiceMenuItem item in EnemyMenuItem)
		{
			item.ProgressVisible = v;
		}
	}

	public void SetChoice(int Choice)
	{
		_firstIndex = Math.Max(_firstIndex, Choice - 2);
		_firstIndex = Math.Min(Choice, _firstIndex);
		UtScrollBar.CurrentIndex = Choice;
		for (var i = 0; i < 3; i++)
		{
			int slot = (i + _firstIndex);
			if (GetTree().CurrentScene is EncounterBattle enc)
			{
				EncounterChoiceMenuItem emi = EnemyMenuItem[i];
				if (slot < enc.Enemys.Count)
				{
					BaseEnemy e = enc.Enemys[slot];
					emi.Text = $"{e.DisplayName}{slot}";
					emi.ProgressMaxValue = e.MaxHp;
					emi.ProgressValue = e.Hp;
					emi.Visible = true;
				}
				else
				{
					emi.Visible = false;
				}
				if (Choice - _firstIndex == i)
				{
					BattlePlayerSoul soul = enc.GetPlayerSoul();
					soul.GlobalTransform = emi.GetSoulTransform();
				}
			}
		}
	}
}
