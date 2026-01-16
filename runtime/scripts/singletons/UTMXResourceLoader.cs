using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal partial class UTMXResourceLoader : UTMXSingleton<UTMXResourceLoader>
{
    public static Resource Load(string resPath)
    {
        if (resPath.StartsWith("res://"))
        {
            return ResourceLoader.Load(resPath);
        }
        else
        {
            string resNewPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{resPath}";
            if (FileAccess.FileExists(resNewPath))
            {
                return ResourceLoader.Load(resNewPath);
            }
            resNewPath = $"res://{resPath}";
            if (FileAccess.FileExists(resNewPath))
            {
                return ResourceLoader.Load(resNewPath);
            }
        }
        return null;
    }

}
