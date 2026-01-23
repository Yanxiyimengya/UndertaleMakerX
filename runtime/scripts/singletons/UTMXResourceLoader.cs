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

}
