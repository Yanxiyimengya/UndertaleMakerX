using Godot;
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
