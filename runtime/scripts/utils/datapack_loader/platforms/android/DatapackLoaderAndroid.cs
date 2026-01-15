using Godot;
using Godot.Collections;
using System;
using System.Text;

public class DatapackLoaderAndroid : DatapackLoader
{

	public override bool LoadPack()
	{
		string packPath = "res://" + EngineProperties.DATAPACK_EXTENSION;
        if (!FileAccess.FileExists(packPath)) return false;
        LoadResourcePack(packPath);
        return true;
	}
}
