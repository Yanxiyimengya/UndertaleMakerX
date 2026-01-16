using Godot;
using Godot.Collections;
using System;

// 引擎配置文件读取类
public partial class UTMXRuntimeProjectConfig : UTMXSingleton<UTMXRuntimeProjectConfig>
{
    public Dictionary<string, Variant> FlatConfigDict { get; private set; } = new Dictionary<string, Variant>();

    public Variant TryGetDefault(string key, Variant defValue = default)
    {
        Variant result;
        FlatConfigDict.TryGetValue(key, out result);
        return result.VariantType == Variant.Type.Nil ? defValue : result;
    }

    public T TryGetDefault<T>(string key, T defValue = default) where T : struct
    {
        if (!FlatConfigDict.TryGetValue(key, out Variant result)) return defValue;
        if (result.VariantType == Variant.Type.Nil) return defValue;
        try
        {
            object value = result.VariantType switch
            {
                Variant.Type.Int => result.AsInt32(),
                Variant.Type.Float => result.AsDouble(),
                Variant.Type.Bool => result.AsBool(),
                Variant.Type.String => result.AsString(),
                _ => defValue
            };
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (InvalidCastException)
        {
            return defValue;
        }
        catch (FormatException)
        {
            return defValue;
        }
    }

    public void LoadConfiguration(string configFilePath)
    {
        FlatConfigDict.Clear();

        if (!FileAccess.FileExists(configFilePath)) return;

        FileAccess file = null;
        try
        {
            file = FileAccess.Open(configFilePath, FileAccess.ModeFlags.Read);
            string configString = file.GetAsText();
            file.Close();

            Variant configVariant = Json.ParseString(configString);
            if (configVariant.VariantType == Variant.Type.Nil) return;

            if (configVariant.As<Dictionary>() is not Dictionary rootDict) return;

            TraverseDict(rootDict, "");
        }
        catch (Exception)
        {
            // 可添加日志输出
        }
        finally
        {
            if (file != null && file.IsOpen())
            {
                file.Close();
            }
        }
    }

    /// <summary>
    /// 递归遍历字典，生成 path/key 格式的扁平键值对
    /// </summary>
    /// <param name="dict">当前遍历的字典</param>
    /// <param name="parentPath">父级路径，空字符串代表根节点</param>
    private void TraverseDict(Dictionary dict, string parentPath)
    {
        foreach (var keyValue in dict)
        {
            string key = keyValue.Key.ToString();
            Variant value = keyValue.Value.As<Variant>();

            string currentPath = string.IsNullOrEmpty(parentPath) ? key : $"{parentPath}/{key}";

            if (value.VariantType == Variant.Type.Dictionary && value.As<Dictionary>() is Dictionary childDict)
            {
                TraverseDict(childDict, currentPath);
            }
            else
            {
                if (FlatConfigDict.ContainsKey(currentPath))
                {
                    FlatConfigDict[currentPath] = value;
                }
                else
                {
                    FlatConfigDict.Add(currentPath, value);
                }
            }
        }
    }
}