using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using Jint.Runtime.Modules;

public sealed class JavaScriptModuleResolver : IModuleLoader
{
    private readonly Dictionary<string, Module> _moduleCache = new();
    private static HashSet<string> visitedModules = new HashSet<string>();

    public static string ResolvePath(string referencingModuleLocation, string specifier)
        => ResolvePathInternal(referencingModuleLocation, specifier);

    /// <summary>
    /// Resolve：将 import specifier 解析成 ResolvedSpecifier
    /// </summary>
    public ResolvedSpecifier Resolve(string referencingModuleLocation, ModuleRequest moduleRequest)
    {
        var specifier = moduleRequest.Specifier;

        GD.Print(referencingModuleLocation, " :::::::: ", specifier);

        // 规则 5：裸模块，保持原样（不加 .js、不拼目录、不转 res://）
        if (IsBareModule(specifier))
        {
            // 关键：裸模块 Key 就用 specifier
            return new ResolvedSpecifier(moduleRequest, specifier, null, SpecifierType.Bare);
        }

        // 规则 1~4：解析出最终 Godot 虚拟路径
        var resolvedPath = ResolvePathInternal(referencingModuleLocation, specifier);
        GD.Print(resolvedPath, " :::::::: ", specifier);

        // 用 Uri 保存位置（Jint 只是拿来当标识/定位，不要求是 file://）
        var uri = new Uri(resolvedPath, UriKind.RelativeOrAbsolute);

        // 这里 SpecifierType 用 Bare（参考你给的 GitHub 示例）
        // 重点在于 Key 要用 uri.ToString() / resolvedPath 这种“唯一绝对定位”
        return new ResolvedSpecifier(moduleRequest, uri.ToString(), uri, SpecifierType.Bare);
    }

    /// <summary>
    /// LoadModule：从 resolved.Uri 加载源码并构造 Module
    /// </summary>
    public Module LoadModule(Jint.Engine engine, ResolvedSpecifier resolved)
    {
        // 裸模块：不在这里加载（应该由 host 注册）
        if (resolved.Uri == null)
        {
            throw new InvalidOperationException(
                $"Bare module '{resolved.Key}' must be provided by host, resolved.Uri is null.");
        }

        var location = resolved.Uri.ToString();

        // 对路径进行标准化
        location = NormalizeVirtualPath(location);

        // 缓存命中检查
        if (_moduleCache.TryGetValue(location, out var cached))
        {
            GD.Print($"Module {location} found in cache.");
            return cached;
        }

        // 读取并加载模块
        var code = ReadAllText(location);
        var prepared = Jint.Engine.PrepareModule(code, location);
        var module = ModuleFactory.BuildSourceTextModule(engine, prepared);

        // 缓存已加载模块
        _moduleCache[location] = module;

        return module;
    }

    // ---------------------------------------------------------------------
    // 路径解析规则（严格按你需求）
    // ---------------------------------------------------------------------

    private static string ResolvePathInternal(string referencingModuleLocation, string specifier)
    {
        // 打印原始路径和引用路径，检查是否为预期的相对路径
        GD.Print($"Resolving {specifier} from {referencingModuleLocation}");

        // 规则 1：res:// 开头 -> 原样返回（补 .js）
        if (specifier.StartsWith("res://", StringComparison.OrdinalIgnoreCase))
            return EnsureJsExtension(specifier);

        // 规则 2：user:// 开头 -> 原样返回（补 .js）
        if (specifier.StartsWith("user://", StringComparison.OrdinalIgnoreCase))
            return EnsureJsExtension(specifier);

        // 规则 3：../ 或 ./ 相对导入 -> 相对 referencingModuleLocation
        if (specifier.StartsWith("./", StringComparison.Ordinal) ||
            specifier.StartsWith("../", StringComparison.Ordinal))
        {
            if (string.IsNullOrEmpty(referencingModuleLocation))
                throw new InvalidOperationException(
                    $"Relative import '{specifier}' requires referencingModuleLocation.");

            var baseDir = GetParentDirectoryPreserveScheme(referencingModuleLocation);
            var combined = CombineVirtualPath(baseDir, specifier);
            combined = NormalizeVirtualPath(combined);
            return EnsureJsExtension(combined);
        }

        // 规则 4：默认根目录映射
        {
            var cleaned = specifier.StartsWith("/", StringComparison.Ordinal)
                ? specifier.Substring(1)
                : specifier;

            var root = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/";
            var loc = CombineVirtualPath(root, cleaned);
            loc = NormalizeVirtualPath(loc);
            return EnsureJsExtension(loc);
        }
    }

    // ---------------------------------------------------------------------
    // 工具函数
    // ---------------------------------------------------------------------

    private static bool IsBareModule(string specifier)
    {
        // 裸模块例子："ABC"
        // 非裸模块例子：res://x user://x ./x ../x /x a/b.js
        if (string.IsNullOrEmpty(specifier))
            return false;

        if (specifier.StartsWith("res://", StringComparison.OrdinalIgnoreCase) ||
            specifier.StartsWith("user://", StringComparison.OrdinalIgnoreCase) ||
            specifier.StartsWith("./", StringComparison.Ordinal) ||
            specifier.StartsWith("../", StringComparison.Ordinal) ||
            specifier.StartsWith("/", StringComparison.Ordinal))
        {
            return false;
        }

        if (specifier.Contains("://", StringComparison.Ordinal))
            return false;

        // Windows 盘符防御
        if (specifier.Length >= 2 && char.IsLetter(specifier[0]) && specifier[1] == ':')
            return false;

        // 有路径分隔符则不是裸模块
        if (specifier.Contains("/", StringComparison.Ordinal) || specifier.Contains("\\", StringComparison.Ordinal))
            return false;

        return true;
    }

    private static string EnsureJsExtension(string path)
    {
        // 如果最后一个 '.' 在最后一个 '/' 之后，则认为已有扩展名
        var lastSlash = path.LastIndexOf('/');
        var lastDot = path.LastIndexOf('.');

        if (lastDot > lastSlash)
            return path;

        return path + ".js";
    }

    /// <summary>
    /// 不使用 Path.GetDirectoryName，避免 res:// 被破坏
    /// </summary>
    private static string GetParentDirectoryPreserveScheme(string fullLocation)
    {
        // fullLocation: res://aaa/bbb/ccc.js -> res://aaa/bbb/
        var lastSlash = fullLocation.LastIndexOf('/');
        if (lastSlash < 0)
            return "";

        return fullLocation.Substring(0, lastSlash + 1);
    }

    private static string CombineVirtualPath(string baseDir, string relativeOrChild)
    {
        if (string.IsNullOrEmpty(baseDir))
            return relativeOrChild;

        if (string.IsNullOrEmpty(relativeOrChild))
            return baseDir;

        // 绝对协议直接返回
        if (relativeOrChild.StartsWith("res://", StringComparison.OrdinalIgnoreCase) ||
            relativeOrChild.StartsWith("user://", StringComparison.OrdinalIgnoreCase))
        {
            return relativeOrChild;
        }

        if (!baseDir.EndsWith("/", StringComparison.Ordinal))
            baseDir += "/";

        // "/xxx" 在这里被视作 baseDir 下的相对路径
        if (relativeOrChild.StartsWith("/", StringComparison.Ordinal))
            relativeOrChild = relativeOrChild.Substring(1);

        return baseDir + relativeOrChild;
    }

    /// <summary>
    /// 处理 ./ 和 ../，并保持 res:// user:// 协议头不被破坏
    /// </summary>
    private static string NormalizeVirtualPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        string scheme = "";
        string rest = path;

        if (path.StartsWith("res://", StringComparison.OrdinalIgnoreCase))
        {
            scheme = "res://";
            rest = path.Substring("res://".Length);
        }
        else if (path.StartsWith("user://", StringComparison.OrdinalIgnoreCase))
        {
            scheme = "user://";
            rest = path.Substring("user://".Length);
        }

        var parts = rest.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var stack = new List<string>(parts.Length);

        foreach (var p in parts)
        {
            if (p == ".")
                continue;

            if (p == "..")
            {
                if (stack.Count > 0)
                    stack.RemoveAt(stack.Count - 1);
                continue;
            }

            stack.Add(p);
        }

        return scheme + string.Join("/", stack);
    }

    private static string ReadAllText(string location)
    {
        using var fa = FileAccess.Open(location, FileAccess.ModeFlags.Read);
        if (fa == null)
        {
            var err = FileAccess.GetOpenError();
            throw new InvalidOperationException($"Failed to open module file '{location}', error={err}");
        }

        var bytes = fa.GetBuffer((long)fa.GetLength());
        return Encoding.UTF8.GetString(bytes);
    }
}
