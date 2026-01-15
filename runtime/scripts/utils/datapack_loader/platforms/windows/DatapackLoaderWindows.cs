using Godot;
using Godot.Collections;
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

		// 搜索可执行文件目录下的独立资源包
		bool found = LoadResourcePack(execBaseDir.PathJoin(execBaseName) + $".{EngineProperties.DATAPACK_EXTENSION}") || 
				LoadResourcePack(execBaseDir.PathJoin(execFileName) + $".{EngineProperties.DATAPACK_EXTENSION}");
		if (!found)
		{
			found = LoadResourcePack(execBaseDir + execBaseName + $".{EngineProperties.DATAPACK_EXTENSION}") ||
				LoadResourcePack(execBaseDir + execFileName + $".{EngineProperties.DATAPACK_EXTENSION}");
		}

		// 搜索内嵌资源包
		if (!found)
		{
			using FileAccess file = FileAccess.Open(execPath, FileAccess.ModeFlags.Read);
			if (file == null) return false;
			ulong fileSize = file.GetLength();

            file.Seek(fileSize - 4);
			uint magic = file.Get32();
			if (magic == GODOT_PACK_HEADER_MAGIC)
			{
				file.Seek(fileSize - 16);
                magic = file.Get32();
				if (magic == UTMX_PACK_HEADER_MAGIC)
				{
					file.Seek(fileSize - 24);
					ulong utmxPckSize = file.Get64();
					ulong utmxPckOffset = fileSize - utmxPckSize - 24;
					found = LoadResourcePack(execPath, (int)utmxPckOffset);
				}
			}
		}
		return found;
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
