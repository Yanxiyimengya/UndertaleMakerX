using Godot;

public abstract class DatapackLoader
{
    public const uint UTMX_PACK_HEADER_MAGIC = 0x4B505455;
    public const uint GODOT_PACK_HEADER_MAGIC = 0x43504447;
    public abstract bool LoadPack();

    public static DatapackLoader GetDatapackLoader(string platform)
    {
        switch (platform)
        {
            case "Windows":
                {
                    return new DatapackLoaderWindows();
                }
                ;
            case "Android":
                {
                    return new DatapackLoaderAndroid();
                }
                ;
            default:
                return null;
        }
    }

    protected bool LoadResourcePack(string pck, int offset = 0)
    {
        return ProjectSettings.LoadResourcePack(pck, true, offset);
    }
}

/*
 * =========  UTMX 数据包  ===========
 * 
 * UTMX数据包 是一个跟随在Godot原始内嵌pck文件末尾的位置
 * 
 * 
 * UTMX 通过伪造一个虚假的PCK末尾实现自定义PCK内嵌
 * Godot会通过Windows的PE头来解析PCK的原始偏移，
 * 如果未找到PE头的pck段，则会在 文件末尾-4字节 的位置查找0x43504447（GD_PCK魔数）
 * 如果找到魔数，则会在 文件末尾-12字节 的位置查找 原始PCK长度
 * 找到PCK长度后，会通过文件末尾位置 - PCK长度 -12字节 的位置定位PCK Offset
 * 
 * Godot原始结构大致是:
 * [原始EXE][Godot原始PCK数据][Godot原始PCK长度][GODOT PCK魔数]
 * 
 * UTMX会将Godot结构改造为:
 * ========================================================================================================================
 * |  x字节  ||     x字节     ||     x字节     ||     8字节     ||    4字节    ||          8字节             ||     4字节    |
 * ========================================================================================================================
 * |[原始EXE] [Godot原始PCK数据][UTMX补丁Pck数据][UTMX补丁数据长度][UTMX PCK魔数][原始PCK长度+UTMX补丁数据长度+4][GODOT PCK魔数]|
 * ========================================================================================================================
 * 
 * 这样，Godot会被UTMX重定位的PCK Offset引导读取到[Godot原始PCK数据]的起始位置
 * 而UTMX运行时会跳过末尾的12字节（跳过伪造的PCK末尾），直接查找后面的UTMX PCK魔数，
 * 若找到，则可以直接读取补丁数据长度，计算UTMX补丁Pck数据位置
 * 
 */
