using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

// 用于管理本地玩家数据
public static class UtmxPlayerDataManager
{
	public static string PlayerName = "FRISK";
	public static double PlayerLv = 1;
	public static double PlayerExp = 0;
	public static double PlayerGold = 0;
	public static double PlayerHp = 20F;
	public static double PlayerMaxHp = 20F;
	public static double PlayerAttack = 0F;
	public static double PlayerDefence = 0F;
	public static double PlayerInvincibleTime = 0.75F;
	public static double MaxInventoryCount = 8;

	public static List<BaseItem> Items = new List<BaseItem>() {};
	public static BaseWeapon Weapon = new BaseWeapon();
	public static BaseArmor Armor = new BaseArmor();

	public static void AddItem(string itemId)
	{
		if (GetItemCount() < MaxInventoryCount)
		{
			if (UtmxGameRegisterDB.TryGetItem(itemId, out BaseItem item))
			{
				Items.Add(item);
			}
		}
	}
	public static void SetItem(string itemId, int slot)
	{
		if (slot > -1 && slot < GetItemCount())
		{
			if (UtmxGameRegisterDB.TryGetItem(itemId, out BaseItem item))
			{
				Items[slot] = item;
			}
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
		if (UtmxBattleManager.IsInBattle())
		{
			UtmxDialogueQueueManager.AppendDialogue(item.UsedText);
		}
	}

	public static void RemoveItem(int slot)
	{
		if (slot < 0 || slot >= Items.Count) return;
		if (slot > -1 && slot < Items.Count)
		{
			Items.RemoveAt(slot);
		}
	}

	public static void Hurt(double value, double invtime = -1)
	{
		if (UtmxBattleManager.IsInBattle())
		{
			UtmxBattleManager.GetBattlePlayerController()?.PlayerSoul?.Hurt(value, invtime);
			if (PlayerHp <= 0)
			{
				UtmxBattleManager.GameOver();
			}
		}
		else
		{
			PlayerHp -= value;
			UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("HURT"));
		}
		PlayerHp = Math.Clamp(PlayerHp, 0, PlayerMaxHp);
	}
	public static void Heal(double value)
	{
		UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("HEAL"));
		PlayerHp += (float)value;
		PlayerHp = Math.Clamp(PlayerHp, 0, PlayerMaxHp);
	}
}
