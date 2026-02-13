using Godot;
using Godot.Collections;
using System;
using System.IO;
using System.Resources;

internal static class UtmxResourceLoader
{
	public static Dictionary<string, Resource> resourceCache = new();
	public static Resource Load(string resPath)
	{
		string resNewPath = ResolvePath(resPath);
		if (string.IsNullOrEmpty(resNewPath)) return null;

		if (resourceCache.TryGetValue(resNewPath, out Resource res) && res != null)
			return res;
		Resource utmxRes = ResourceLoader.Load(resNewPath, "", ResourceLoader.CacheMode.Ignore);
		if (utmxRes != null) 
			resourceCache.Add(resNewPath, utmxRes);
		return utmxRes;
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
		if (string.IsNullOrEmpty(path)) return "";
		if (Path.IsPathRooted(path) || path.StartsWith("res://") || path.StartsWith("uid://"))
		{
			return path;
		}
		string resPackPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{path}";
		return resPackPath;
	}

	public static void ClearCache()
	{
		resourceCache.Clear();

	}
}
