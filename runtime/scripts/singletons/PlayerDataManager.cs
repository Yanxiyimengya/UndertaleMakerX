using System;
using System.Collections.Generic;
using Godot;

// 用于管理本地玩家数据
public partial class PlayerDataManager
{

	private static readonly Lazy<PlayerDataManager> _instance =
		new Lazy<PlayerDataManager>(() => new PlayerDataManager());
	private PlayerDataManager() { }
	public static PlayerDataManager Instance => _instance.Value;

	public string PlayerName = "FRISK";
	public int PlayerLv = 19;
	public float PlayerHp = 92F;
	public float PlayerMaxHp = 92F;
    public float PlayerAttack = 0F;
    public float PlayerDefence = 0F;
	public float PlayerInvincibleTime = 0.005F;

    public List<BaseItem> Items = new List<BaseItem>()
	{
		new BaseItem(),
		new BaseItem(),
        new BaseItem(),
        new BaseItem(),
        new BaseItem(),
        new BaseItem(),
        new BaseItem(),
    };
	public BaseWeapon Weapon = new BaseWeapon();

	public int GetItemCount()
	{
		return Items.Count;
	}

	public bool TryGetItem(int slot, out BaseItem item)
	{
		item = null;
		if (slot > -1 && slot < Items.Count)
		{
			item = Items[slot];
			return true;
		}
		return false;
    }
    public void UseItem(int slot)
    {
        BaseItem item = Items[slot];
        item.Slot = slot;
        item.OnUseSelected();
    }

    public void AddItem(BaseItem item)
	{
		if (item != null)
		{
			Items.Add(item);
		}
	}
}
