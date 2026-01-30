using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

// 用于管理本地玩家数据
public static class UtmxPlayerDataManager
{
	public static string PlayerName = "FRISK";
	public static int PlayerLv = 19;
	public static float PlayerHp = 92F;
	public static float PlayerMaxHp = 92F;
	public static float PlayerAttack = 0F;
	public static float PlayerDefence = 0F;
	public static float PlayerInvincibleTime = 0.025F * 30F;

	public static List<BaseItem> Items = new List<BaseItem>() {};
	public static BaseWeapon Weapon = new BaseWeapon();
	public static BaseArmor Armor = new BaseArmor();

	public static void AddItem(string itemId)
	{
		if (GameRegisterDB.TryGetItem(itemId, out BaseItem item))
		{
			Items.Add(item);
		}
	}
	private static bool TryGetItem(int slot, out BaseItem item)
	{
		item = null;
		if (slot > -1 && slot < Items.Count)
		{
			item = Items[slot];
			return true;
		}
		return false;
	}

	public static int GetItemCount()
	{
		return Items.Count;
	}


	public static void UseItem(int slot)
	{
		BaseItem item = Items[slot];
		item.ItemSlot = slot;
		item._OnUseSelected();
	}
	public static void RemoveItem(int slot)
	{
		if (slot > -1 && slot < Items.Count)
		{
			Items.RemoveAt(slot);
		}
	}
}
