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
    public ResolvedSpecifier Resolve(string referencingModuleLocation, ModuleRequest moduleRequest)
    {
        var specifier = moduleRequest.Specifier;

        if (IsBareModule(specifier))
        {
            return new ResolvedSpecifier(moduleRequest, specifier, null, SpecifierType.Bare);
        }

        var resolvedPath = ResolvePathInternal(referencingModuleLocation, specifier);

        var uri = new Uri(resolvedPath, UriKind.RelativeOrAbsolute);
        return new ResolvedSpecifier(moduleRequest, uri.ToString(), uri, SpecifierType.Bare);
    }

    public Module LoadModule(Jint.Engine engine, ResolvedSpecifier resolved)
    {
        if (resolved.Uri == null)
        {
            throw new InvalidOperationException(
                $"Bare module '{resolved.Key}' must be provided by host, resolved.Uri is null.");
        }

        var location = resolved.Uri.ToString();

        location = NormalizeVirtualPath(location);

        if (_moduleCache.TryGetValue(location, out var cached))
        {
            return cached;
        }

        var code = ReadAllText(location);
        var prepared = Jint.Engine.PrepareModule(code, location);
        var module = ModuleFactory.BuildSourceTextModule(engine, prepared);

        _moduleCache[location] = module;

        return module;
    }

    private static string ResolvePathInternal(string referencingModuleLocation, string specifier)
    {
        if (specifier.StartsWith("res://", StringComparison.OrdinalIgnoreCase))
            return EnsureJsExtension(specifier);

        if (specifier.StartsWith("user://", StringComparison.OrdinalIgnoreCase))
            return EnsureJsExtension(specifier);

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

    private static bool IsBareModule(string specifier)
    {
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

        if (specifier.Length >= 2 && char.IsLetter(specifier[0]) && specifier[1] == ':')
            return false;

        if (specifier.Contains("/", StringComparison.Ordinal) || specifier.Contains("\\", StringComparison.Ordinal))
            return false;

        return true;
    }

    private static string EnsureJsExtension(string path)
    {
        if (! path.EndsWith(".js"))
            return path + ".js";
        return path;
    }

    private static string GetParentDirectoryPreserveScheme(string fullLocation)
    {
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

        if (relativeOrChild.StartsWith("res://", StringComparison.OrdinalIgnoreCase) ||
            relativeOrChild.StartsWith("user://", StringComparison.OrdinalIgnoreCase))
        {
            return relativeOrChild;
        }

        if (!baseDir.EndsWith("/", StringComparison.Ordinal))
            baseDir += "/";

        if (relativeOrChild.StartsWith("/", StringComparison.Ordinal))
            relativeOrChild = relativeOrChild.Substring(1);

        return baseDir + relativeOrChild;
    }

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
        using var access = FileAccess.Open(location, FileAccess.ModeFlags.Read);
        if (access == null)
        {
            var err = FileAccess.GetOpenError();
            throw new InvalidOperationException($"Failed to open module file '{location}', error={err}");
        }

        var bytes = access.GetBuffer((long)access.GetLength());
        access.Close();
        return Encoding.UTF8.GetString(bytes);
    }
}
