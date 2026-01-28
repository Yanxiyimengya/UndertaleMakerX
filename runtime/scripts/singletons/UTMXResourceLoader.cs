using Godot;
using System;
using System.IO;

internal partial class UtmxResourceLoader
{

    private static readonly Lazy<UtmxResourceLoader> _instance =
        new Lazy<UtmxResourceLoader>(() => new UtmxResourceLoader());
    private UtmxResourceLoader() { }
    public static UtmxResourceLoader Instance => _instance.Value;

    public static Resource Load(string resPath)
    {
        if (string.IsNullOrEmpty(resPath)) return null;
        if (resPath.StartsWith("res://"))
        {
            if (Godot.FileAccess.FileExists(resPath))
                return ResourceLoader.Load(resPath);
        }
        else
        {
            string resNewPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{resPath}";
            if (Godot.FileAccess.FileExists(resNewPath))
            {
                return ResourceLoader.Load(resNewPath);
            }
            resNewPath = $"res://{resPath}";
            if (Godot.FileAccess.FileExists(resNewPath))
            {
                return ResourceLoader.Load(resNewPath);
            }
        }
        return null;
    }

    public Godot.FileAccess OpenFile(string filePath, Godot.FileAccess.ModeFlags flags)
    {
        if (string.IsNullOrEmpty(filePath)) return null;
        if (filePath.StartsWith("res://"))
        {
            if (Godot.FileAccess.FileExists(filePath))
                return Godot.FileAccess.Open(filePath, flags);
        }
        else
        {
            string resNewPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{filePath}";
            if (Godot.FileAccess.FileExists(resNewPath))
            {
                return Godot.FileAccess.Open(resNewPath, flags);
            }
            resNewPath = $"res://{filePath}";
            if (Godot.FileAccess.FileExists(resNewPath))
            {
                return Godot.FileAccess.Open(resNewPath, flags);
            }
        }
        return null;
    }

    public static string ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (Path.IsPathRooted(path) || path.StartsWith("res://"))
        {
            return path;
        }
        string resPackPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{path}";
        return resPackPath;
    }

}
