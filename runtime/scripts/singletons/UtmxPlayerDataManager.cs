using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

// 用于管理本地玩家数据
public static class UtmxPlayerDataManager
{
	public static string PlayerName = "FRISK";
	public static double PlayerLv = 19;
	public static double PlayerExp = 0;
	public static double PlayerGold = 0;
	public static double PlayerHp = 92F;
	public static double PlayerMaxHp = 92F;
	public static double PlayerAttack = 0F;
	public static double PlayerDefence = 0F;
	public static double PlayerInvincibleTime = 0.025F * 30F;

	public static List<BaseItem> Items = new List<BaseItem>() {};
	public static BaseWeapon Weapon = new BaseWeapon();
	public static BaseArmor Armor = new BaseArmor();

	public static void AddItem(string itemId)
	{
		if (UtmxGameRegisterDB.TryGetItem(itemId, out BaseItem item))
		{
			Items.Add(item);
		}
	}
	public static BaseItem GetItemAt(int slot)
	{
		if (TryGetItem(slot, out BaseItem result))
		{
			return result;
        }
		return null;
	}

    private static bool TryGetItem(int slot, out BaseItem item)
	{
		item = null;
		if (slot < 0 || slot >= Items.Count) return false;
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
		if (slot < 0 || slot >= Items.Count) return;
		BaseItem item = Items[slot];
		item.ItemSlot = slot;
		item._OnUseSelected();
	}

	public static void RemoveItem(int slot)
	{
		if (slot < 0 || slot >= Items.Count) return;
		if (slot > -1 && slot < Items.Count)
		{
			Items.RemoveAt(slot);
		}
	}



	public static void Hurt(double value)
	{
		UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("HURT"));
		if (UtmxBattleManager.IsInBattle())
		{
			UtmxBattleManager.GetBattlePlayerController()?.PlayerSoul?.Hurt(value);
			if (PlayerHp <= 0)
			{
				UtmxBattleManager.GameOver();
			}
		}
		else
		{
			PlayerHp -= value;
		}

	}
	public static void Heal(double value)
	{
		UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("HEAL"));
		PlayerHp += (float)value;
	}
}
