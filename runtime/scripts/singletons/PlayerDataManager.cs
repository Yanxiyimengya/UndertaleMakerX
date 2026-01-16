using System;
using System.Collections.Generic;
using Godot;

// 用于管理本地玩家数据
public partial class PlayerDataManager : UTMXSingleton<PlayerDataManager>
{
    public string PlayerName = "FRISK";
    public int PlayerLv = 1;
    public float PlayerHp = 20F;
    public float PlayerMaxHp = 20F;

    public List<BaseItem> Items = new List<BaseItem>();

    public int GetItemCount()
    {
        return Items.Count;
    }
}
