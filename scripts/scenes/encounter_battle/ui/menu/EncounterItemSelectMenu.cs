using Godot;
using System;

public partial class EncounterItemChoiceMenu : BaseEncounterMenu
{
	[Export]
	UndertaleStyleScrollBar scrollBar;
	
	public override void UIVisible()
	{
		PlayerDataManager.Instance.GetItemCount();
	}
	public override void UIHidden()
	{

	}
}
