using Godot;

[GlobalClass]
public partial class EncounterChoicePageMenu : EncounterChoiceMenu
{
    [Export]
    public TextTyper PageTextTyper;

    public override void UIVisible() { }
    public override void UIHidden() { }

    public void HpBarSetVisible(bool v)
    {
        foreach (EncounterChoiceMenuItem item in MenuItems)
        {
            item.ProgressVisible = v;
        }
    }

    public async override void SetChoice(int choice)
    {
        if (choice < 0 || choice >= GetItemCount())
        {
            return;
        }
        int page = choice / MenuItems.Count;
        _currentChoice = choice;

        if (GetItemCount() > MenuItems.Count)
        {
            PageTextTyper.Start($"PAGE {page + 1}");
            PageTextTyper.Visible = true;
        }
        else
        {
            PageTextTyper.Visible = false;
        }

        for (var i = 0; i < MenuItems.Count; i++)
        {
            int slot = (page * MenuItems.Count) + i;
            EncounterChoiceMenuItem menuItem = MenuItems[i];
            if (slot >= GetItemCount())
            {
                menuItem.Visible = false;
                continue;
            }
            ChoiceItem choiceItem = GetItem(slot);
            menuItem.Visible = true;
            menuItem.Text = choiceItem.ItemDisplayName;
            if (menuItem.ProgressVisible)
            {
                menuItem.ProgressMaxValue = choiceItem.MaxValue;
                menuItem.ProgressValue = choiceItem.Value;
            }
        }


        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        int soulSelectIndex = choice - (page * MenuItems.Count);
        if (soulSelectIndex < 4)
        {
            var menuItem = MenuItems[soulSelectIndex];
            UtmxBattleManager.Instance.GetPlayerSoul().GlobalTransform = menuItem.GetSoulTransform();
        }
    }
}
