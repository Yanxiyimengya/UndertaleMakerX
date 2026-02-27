using Godot;
using System;
using System.Text;

public class DatapackLoaderLinux : DatapackLoader
{
    private const byte ELF_CLASS_32 = 1;
    private const byte ELF_CLASS_64 = 2;
    private const byte ELF_DATA_LE = 1;
    private const byte ELF_DATA_BE = 2;
    private const ushort SHN_XINDEX = 0xFFFF;
    private const int UTMX_TAIL_SIZE = 24;

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
            if (FileAccess.FileExists(path) && LoadResourcePack(path, 0))
            {
                return true;
            }
        }

        using var file = FileAccess.Open(execPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            return false;
        }

        if (!TryFindGodotPackHeader(file, out _))
        {
            return false;
        }

        return TryLoadEmbeddedUtmxPack(file, execPath);
    }

    private bool TryLoadEmbeddedUtmxPack(FileAccess file, string execPath)
    {
        ulong fileSize = file.GetLength();
        if (fileSize < UTMX_TAIL_SIZE)
        {
            return false;
        }

        if (!TryReadUInt32At(file, fileSize - 4, out uint godotTailMagic) ||
            godotTailMagic != GODOT_PACK_HEADER_MAGIC)
        {
            return false;
        }

        if (!TryReadUInt32At(file, fileSize - 16, out uint utmxMagic) ||
            utmxMagic != UTMX_PACK_HEADER_MAGIC)
        {
            return false;
        }

        if (!TryReadUInt64At(file, fileSize - 24, out ulong utmxPackSize))
        {
            return false;
        }
        if (utmxPackSize == 0 || utmxPackSize > fileSize - UTMX_TAIL_SIZE)
        {
            return false;
        }

        ulong utmxPackOffset = fileSize - utmxPackSize - UTMX_TAIL_SIZE;
        if (!TryReadUInt32At(file, utmxPackOffset, out uint utmxPackMagic) ||
            utmxPackMagic != GODOT_PACK_HEADER_MAGIC)
        {
            return false;
        }

        if (utmxPackOffset > int.MaxValue)
        {
            return false;
        }

        return LoadResourcePack(execPath, (int)utmxPackOffset);
    }

    private bool TryFindGodotPackHeader(FileAccess file, out ulong packHeaderOffset)
    {
        packHeaderOffset = 0;
        ulong embeddedPckOffset = GetEmbeddedPckSectionOffset(file);

        if (embeddedPckOffset != 0)
        {
            for (int i = 0; i < 8; i++)
            {
                ulong probeOffset = embeddedPckOffset + (ulong)i;
                if (!TryReadUInt32At(file, probeOffset, out uint magic))
                {
                    break;
                }
                if (magic == GODOT_PACK_HEADER_MAGIC)
                {
                    packHeaderOffset = probeOffset;
                    return true;
                }
            }
        }

        ulong fileSize = file.GetLength();
        if (fileSize < 12)
        {
            return false;
        }

        if (!TryReadUInt32At(file, fileSize - 4, out uint tailMagic) ||
            tailMagic != GODOT_PACK_HEADER_MAGIC)
        {
            return false;
        }

        if (!TryReadUInt64At(file, fileSize - 12, out ulong embeddedPackSize))
        {
            return false;
        }
        if (embeddedPackSize > fileSize - 12)
        {
            return false;
        }

        ulong fallbackOffset = fileSize - embeddedPackSize - 12;
        if (!TryReadUInt32At(file, fallbackOffset, out uint fallbackMagic) ||
            fallbackMagic != GODOT_PACK_HEADER_MAGIC)
        {
            return false;
        }

        packHeaderOffset = fallbackOffset;
        return true;
    }

    private ulong GetEmbeddedPckSectionOffset(FileAccess file)
    {
        try
        {
            if (!TryReadBytesAt(file, 0, 16, out byte[] ident))
            {
                return 0;
            }
            if (ident[0] != 0x7F || ident[1] != (byte)'E' || ident[2] != (byte)'L' || ident[3] != (byte)'F')
            {
                return 0;
            }

            byte elfClass = ident[4];
            byte elfData = ident[5];
            if ((elfClass != ELF_CLASS_32 && elfClass != ELF_CLASS_64) ||
                (elfData != ELF_DATA_LE && elfData != ELF_DATA_BE))
            {
                return 0;
            }
            bool littleEndian = elfData == ELF_DATA_LE;

            ulong sectionHeaderPos;
            ushort sectionHeaderSize;
            ushort sectionHeaderCount;
            ushort stringSectionIndex;

            if (elfClass == ELF_CLASS_32)
            {
                if (!TryReadUInt32At(file, 0x20, littleEndian, out uint shPos32))
                {
                    return 0;
                }
                sectionHeaderPos = shPos32;
                if (!TryReadUInt16At(file, 0x2E, littleEndian, out sectionHeaderSize))
                {
                    return 0;
                }
                if (!TryReadUInt16At(file, 0x30, littleEndian, out sectionHeaderCount))
                {
                    return 0;
                }
                if (!TryReadUInt16At(file, 0x32, littleEndian, out stringSectionIndex))
                {
                    return 0;
                }
            }
            else
            {
                if (!TryReadUInt64At(file, 0x28, littleEndian, out sectionHeaderPos))
                {
                    return 0;
                }
                if (!TryReadUInt16At(file, 0x3A, littleEndian, out sectionHeaderSize))
                {
                    return 0;
                }
                if (!TryReadUInt16At(file, 0x3C, littleEndian, out sectionHeaderCount))
                {
                    return 0;
                }
                if (!TryReadUInt16At(file, 0x3E, littleEndian, out stringSectionIndex))
                {
                    return 0;
                }
            }

            if (sectionHeaderPos == 0 || sectionHeaderSize == 0 || sectionHeaderCount == 0)
            {
                return 0;
            }

            if (stringSectionIndex == SHN_XINDEX)
            {
                ulong xIndexOffset = sectionHeaderPos + (elfClass == ELF_CLASS_32 ? 0x1CUL : 0x2CUL);
                if (!TryReadUInt32At(file, xIndexOffset, littleEndian, out uint xIndex))
                {
                    return 0;
                }
                stringSectionIndex = (ushort)xIndex;
            }
            if (stringSectionIndex >= sectionHeaderCount)
            {
                return 0;
            }

            ulong stringSectionHeader = sectionHeaderPos + (ulong)stringSectionIndex * sectionHeaderSize;
            ulong stringDataOffsetField = stringSectionHeader + (elfClass == ELF_CLASS_32 ? 0x10UL : 0x18UL);
            ulong stringDataSizeField = stringSectionHeader + (elfClass == ELF_CLASS_32 ? 0x14UL : 0x20UL);

            if (!TryReadElfNativeWordAt(file, stringDataOffsetField, elfClass, littleEndian, out ulong stringDataOffset))
            {
                return 0;
            }
            if (!TryReadElfNativeWordAt(file, stringDataSizeField, elfClass, littleEndian, out ulong stringDataSize))
            {
                return 0;
            }
            if (stringDataSize == 0 || stringDataSize > int.MaxValue)
            {
                return 0;
            }

            if (!TryReadBytesAt(file, stringDataOffset, (int)stringDataSize, out byte[] stringData))
            {
                return 0;
            }

            ulong sectionTableSpan = (ulong)sectionHeaderSize * sectionHeaderCount;
            ulong fileSize = file.GetLength();
            if (sectionHeaderPos > fileSize || sectionTableSpan > fileSize - sectionHeaderPos)
            {
                return 0;
            }

            for (ushort i = 0; i < sectionHeaderCount; i++)
            {
                ulong sectionHeaderOffset = sectionHeaderPos + (ulong)i * sectionHeaderSize;
                if (!TryReadUInt32At(file, sectionHeaderOffset, littleEndian, out uint sectionNameOffset))
                {
                    return 0;
                }
                if (sectionNameOffset >= stringData.Length)
                {
                    continue;
                }

                string sectionName = ReadAsciiCString(stringData, (int)sectionNameOffset);
                if (!string.Equals(sectionName, "pck", StringComparison.Ordinal))
                {
                    continue;
                }

                ulong sectionOffsetField = sectionHeaderOffset + (elfClass == ELF_CLASS_32 ? 0x10UL : 0x18UL);
                if (!TryReadElfNativeWordAt(file, sectionOffsetField, elfClass, littleEndian, out ulong pckSectionOffset))
                {
                    return 0;
                }
                return pckSectionOffset;
            }
        }
        catch (Exception)
        {
            return 0;
        }

        return 0;
    }

    private bool TryReadElfNativeWordAt(FileAccess file, ulong offset, byte elfClass, bool littleEndian, out ulong value)
    {
        value = 0;

        if (elfClass == ELF_CLASS_32)
        {
            if (!TryReadUInt32At(file, offset, littleEndian, out uint value32))
            {
                return false;
            }
            value = value32;
            return true;
        }

        if (elfClass == ELF_CLASS_64)
        {
            return TryReadUInt64At(file, offset, littleEndian, out value);
        }

        return false;
    }

    private bool TryReadUInt16At(FileAccess file, ulong offset, bool littleEndian, out ushort value)
    {
        value = 0;
        if (!TryReadBytesAt(file, offset, 2, out byte[] data))
        {
            return false;
        }
        value = littleEndian
            ? (ushort)(data[0] | (data[1] << 8))
            : (ushort)((data[0] << 8) | data[1]);
        return true;
    }

    private bool TryReadUInt32At(FileAccess file, ulong offset, out uint value)
    {
        return TryReadUInt32At(file, offset, true, out value);
    }

    private bool TryReadUInt32At(FileAccess file, ulong offset, bool littleEndian, out uint value)
    {
        value = 0;
        if (!TryReadBytesAt(file, offset, 4, out byte[] data))
        {
            return false;
        }

        if (littleEndian)
        {
            value = data[0]
                | ((uint)data[1] << 8)
                | ((uint)data[2] << 16)
                | ((uint)data[3] << 24);
        }
        else
        {
            value = ((uint)data[0] << 24)
                | ((uint)data[1] << 16)
                | ((uint)data[2] << 8)
                | data[3];
        }
        return true;
    }

    private bool TryReadUInt64At(FileAccess file, ulong offset, out ulong value)
    {
        return TryReadUInt64At(file, offset, true, out value);
    }

    private bool TryReadUInt64At(FileAccess file, ulong offset, bool littleEndian, out ulong value)
    {
        value = 0;
        if (!TryReadBytesAt(file, offset, 8, out byte[] data))
        {
            return false;
        }

        if (littleEndian)
        {
            for (int i = 0; i < 8; i++)
            {
                value |= (ulong)data[i] << (8 * i);
            }
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                value = (value << 8) | data[i];
            }
        }
        return true;
    }

    private bool TryReadBytesAt(FileAccess file, ulong offset, int length, out byte[] data)
    {
        data = Array.Empty<byte>();
        if (length < 0)
        {
            return false;
        }

        ulong fileSize = file.GetLength();
        if (offset > fileSize || (ulong)length > fileSize - offset)
        {
            return false;
        }

        file.Seek(offset);
        data = file.GetBuffer(length);
        return data.Length == length;
    }

    private string ReadAsciiCString(byte[] data, int offset)
    {
        int end = offset;
        while (end < data.Length && data[end] != 0)
        {
            end++;
        }
        return Encoding.ASCII.GetString(data, offset, end - offset);
    }
}
