using Godot;
using System;
using System.Text;

public class DatapackLoaderWindows : DatapackLoader
{
    public override bool LoadPack()
    {
        string execPath = OS.GetExecutablePath();
        string execBaseDir = execPath.GetBaseDir();
        string execFileName = execPath.GetFile();
        string execBaseName = execFileName.GetBaseName();

        string[] searchPaths = {
            execBaseDir.PathJoin(execBaseName + "." + EngineProperties.DATAPACK_EXTENSION),
            execBaseDir.PathJoin(execFileName + "." + EngineProperties.DATAPACK_EXTENSION)
        };
        foreach (var path in searchPaths)
        {
            if (FileAccess.FileExists(path))
            {
                if (LoadResourcePack(path, 0))
                {
                    return true;
                }
            }
        }
        using var file = FileAccess.Open(execPath, FileAccess.ModeFlags.Read);
        if (file == null) return false;

        ulong fileSize = file.GetLength();
        if (fileSize < 24) return false;

        file.Seek(fileSize - 4);
        uint godotMagic = file.Get32();

        if (godotMagic == GODOT_PACK_HEADER_MAGIC)
        {
            file.Seek(fileSize - 16);
            uint utmxMagic = file.Get32();

            if (utmxMagic == UTMX_PACK_HEADER_MAGIC)
            {
                file.Seek(fileSize - 24);
                ulong utmxPckSize = file.Get64();
                long utmxPckOffset = (long)(fileSize - utmxPckSize - 24);

                if (LoadResourcePack(execPath, (int)utmxPckOffset))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private ulong GetCustomSectionOffset(FileAccess fileAccess, string sectionName)
    {
        try
        {
            fileAccess.Seek(0x3C);
            uint peHeaderOffset = fileAccess.Get32();
            fileAccess.Seek(peHeaderOffset);
            uint peSignature = fileAccess.Get32();
            if (peSignature != 0x00004550) return 0;
            fileAccess.Seek(peHeaderOffset + 6);
            ushort numberOfSections = fileAccess.Get16();
            fileAccess.Seek(peHeaderOffset + 20);
            ushort sizeOfOptionalHeader = fileAccess.Get16();
            ulong sectionTableOffset = peHeaderOffset + 24 + sizeOfOptionalHeader;
            fileAccess.Seek(sectionTableOffset);
            for (int i = 0; i < numberOfSections; i++)
            {
                ulong sectionHeaderStart = fileAccess.GetPosition();
                byte[] nameBytes = fileAccess.GetBuffer(8);
                string currentSectionName = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');
                if (currentSectionName == sectionName)
                {
                    fileAccess.Seek(sectionHeaderStart + 20);
                    uint rawDataPointer = fileAccess.Get32();
                    return rawDataPointer;
                }
                fileAccess.Seek(sectionHeaderStart + 40);
            }
            return 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

}
