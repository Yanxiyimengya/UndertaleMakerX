using Godot;
using System;
using System.Collections.Generic;

public partial class BattleMenuManager : TabContainer
{
	private Dictionary<string, BaseEncounterMenu> menuList = new Dictionary<string, BaseEncounterMenu>();
	private String prevMenuName = "";

	public override void _Ready()
	{
		Visible = false;
		foreach (Node childNode in this.GetChildren())
		{
			if (!(childNode is BaseEncounterMenu menu)) continue;
			menuList.Add(menu.Name, menu);
		}
	}

	public async System.Threading.Tasks.Task OpenMenu(string menuName)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		if (menuList.TryGetValue(prevMenuName, out BaseEncounterMenu prevMenu))
		{
			prevMenu.UIHidden();
		}
		if (menuList.TryGetValue(menuName, out BaseEncounterMenu menu))
		{
			prevMenuName = menuName;
			menu.UIVisible();
			menu.Visible = true;
		}
		Visible = true;
	}
}
