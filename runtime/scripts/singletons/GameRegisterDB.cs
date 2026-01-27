using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class GameRegisterDB : Node
{
    private GameRegisterDB() { 
    }

    private static readonly Lazy<GameRegisterDB> _instance =
        new Lazy<GameRegisterDB>(() => new GameRegisterDB());
    public static GameRegisterDB Instance => _instance.Value;

    private Dictionary<string, BaseItem> _itemDB = new();
    private Dictionary<string, BaseEnemy> _enemyDB = new();
    // TODO: add more dbs
    // TODO: 修改注册数据为对应包装类，支持元数据

}
