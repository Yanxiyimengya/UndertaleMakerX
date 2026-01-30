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
            { "__input" , typeof(UtmxInputManager)},

            { "__UtmxItem" , typeof(JavaScriptItemProxy)},
            { "__UtmxEnemy" , typeof(JavaScriptEnemyProxy)},
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
