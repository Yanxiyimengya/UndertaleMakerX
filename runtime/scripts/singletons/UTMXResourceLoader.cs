using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal partial class UTMXResourceLoader
{

	private static readonly Lazy<UTMXResourceLoader> _instance =
		new Lazy<UTMXResourceLoader>(() => new UTMXResourceLoader());
	private UTMXResourceLoader() { }
	public static UTMXResourceLoader Instance => _instance.Value;

	public static Resource Load(string resPath)
	{
		if (string.IsNullOrEmpty(resPath)) return null;
		if (resPath.StartsWith("res://"))
		{
			if (FileAccess.FileExists(resPath))
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

	public FileAccess OpenFile(string filePath, FileAccess.ModeFlags flags)
	{
        if (string.IsNullOrEmpty(filePath)) return null;
        if (filePath.StartsWith("res://"))
        {
			if (FileAccess.FileExists(filePath))
				return FileAccess.Open(filePath, flags);
        }
        else
        {
            string resNewPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{filePath}";
            if (FileAccess.FileExists(resNewPath))
            {
                return FileAccess.Open(resNewPath, flags);
            }
            resNewPath = $"res://{filePath}";
            if (FileAccess.FileExists(resNewPath))
            {
                return FileAccess.Open(resNewPath, flags);
            }
        }
        return null;
    }

	public static string ResolvePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        string resPackPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{path}";
        if (FileAccess.FileExists(resPackPath))
            return resPackPath;
        return path;
    }

}
