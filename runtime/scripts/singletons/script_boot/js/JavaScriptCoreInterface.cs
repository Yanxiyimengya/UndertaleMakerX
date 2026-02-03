using Godot;
using System;
using System.Collections.Generic;

// JS全局程序包模块
public class JavaScriptCoreInterface
{
    public static string ModuleName = "__UTMX";

    public static Dictionary<string, Type> _GetCoreExportTypes()
    {
        return new Dictionary<string, Type>()
        {
            { "__logger" , typeof(UtmxLogger)},
            { "__audio_player" , typeof(UtmxGlobalStreamPlayer) },
            { "__player_data_manager" , typeof(UtmxPlayerDataManager)},
            { "__game_register_db" , typeof(UtmxGameRegisterDB)},
            { "__game_manager" , typeof(UtmxGameManager)},
            { "__battle_manager" , typeof(UtmxBattleManager)},
            { "__input" , typeof(UtmxInputManager)},

            { "__Object" , typeof(JavaScriptObject)},
            { "__UtmxItem" , typeof(JavaScriptItemProxy)},
            { "__UtmxEnemy" , typeof(JavaScriptEnemyProxy)},
            { "__UtmxEncounter" , typeof(JavaScriptEncounterProxy)},
            { "__BattleTurn" , typeof(JavaScriptBattleTurnProxy)},
            { "__BattleProjectile" , typeof(JavaScriptBattleProjectileProxy)},

            { "__Vector2" , typeof(Vector2)},
            { "__Vector3" , typeof(Vector3)},
            { "__Vector4" , typeof(Vector4)},
        };
    }
    public static Dictionary<string, string> _GetInterfaceExportScripts()
    {
        return new Dictionary<string, string>()
        {
            { "UTMX", "res://scripts/js/utmx_main.wapper.js"},
        };
    }
}
