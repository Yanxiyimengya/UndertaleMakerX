using Godot;
public class DatapackLoaderAndroid : DatapackLoader
{

    public override bool LoadPack()
    {
        string packPath = "res://".PathJoin("utmx." + EngineProperties.DATAPACK_EXTENSION);
        if (!FileAccess.FileExists(packPath)) return false;
        LoadResourcePack(packPath);
        return true;
    }
}
