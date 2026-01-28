using Godot;
using System;
using System.Collections.Generic;

// JS全局主程序包
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
