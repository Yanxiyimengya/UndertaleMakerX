using Godot;
using System;
using System.Collections.Generic;

public partial class BattleMenuManager : TabContainer
{
	
	private Dictionary<string, BaseEncounterMenu> menuList = new Dictionary<string, BaseEncounterMenu>();
	private String prevMenuName = "";

	public override void _Ready()
	{
		foreach (Node childNode in this.GetChildren())
		{
			if (!(childNode is BaseEncounterMenu menu)) continue;
			menuList.Add(menu.Name, menu);
		}
	}

	public async System.Threading.Tasks.Task OpenMenu(string menuName)
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		Visible = true;
		if (menuList.TryGetValue(prevMenuName, out BaseEncounterMenu prevMenu))
		{
			prevMenu.Visible = false;
			prevMenu.UIHidden();
		}
		if (menuList.TryGetValue(menuName, out BaseEncounterMenu menu))
		{
			prevMenuName = menuName;
			menu.Visible = true;
			menu.UIVisible();
		}
	}
}
