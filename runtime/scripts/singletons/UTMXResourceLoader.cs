using Godot;
using System;
using System.IO;
using System.Resources;

internal static class UtmxResourceLoader
{
    public static Resource Load(string resPath)
    {
        if (string.IsNullOrEmpty(resPath)) return null;
        string resNewPath = ResolvePath(resPath);
        if (ResourceLoader.Exists(resNewPath))
        {
            return ResourceLoader.Load(resNewPath);
        }
        else
        {
            UtmxLogger.Log(TranslationServer.Translate($"Resource file not found: {resPath}"));
        }
        return null;
    }

    public static Godot.FileAccess OpenFile(string filePath, Godot.FileAccess.ModeFlags flags)
    {
        if (string.IsNullOrEmpty(filePath)) return null;
        filePath = ResolvePath(filePath);
        if (Godot.FileAccess.FileExists(filePath))
        {
            return Godot.FileAccess.Open(filePath, flags);
        }
        return null;
    }

    public static string ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (Path.IsPathRooted(path) || path.StartsWith("res://") || path.StartsWith("uid://"))
        {
            return path;
        }
        string resPackPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{path}";
        return resPackPath;
    }

}
