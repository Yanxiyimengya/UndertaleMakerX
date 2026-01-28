using Godot;
using Jint.Runtime.Modules;
using System;
using System.Collections.Generic;
using System.IO;

public class JavaScriptModuleResolver : IModuleLoader
{
    private Dictionary<string, Module> _moduleCache = new Dictionary<string, Module>();

    public Module LoadModule(Jint.Engine engine, ResolvedSpecifier resolved)
    {
        string source = resolved.Uri.ToString();
        if (_moduleCache.TryGetValue(source, out Module cachedModule))
        {
            return cachedModule;
        }
        Godot.FileAccess file = Godot.FileAccess.Open(source, Godot.FileAccess.ModeFlags.Read);
        string code = file.GetBuffer((long)file.GetLength()).GetStringFromUtf8();
        file.Close();
        Module buildModule = ModuleFactory.BuildSourceTextModule(engine, Jint.Engine.PrepareModule(code, source));
        _moduleCache[source] = buildModule;

        return buildModule;
    }

    public ResolvedSpecifier Resolve(string referencingModuleLocation, ModuleRequest moduleRequest)
    {
        string specifier = moduleRequest.Specifier;
        string path = ResolvePath(specifier, referencingModuleLocation);
        GD.Print("包源路径:", specifier, "|引用文件路径:", referencingModuleLocation);
        GD.Print("解析路径", path);
        GD.Print("=-=-=-");
        Uri resolved;
        if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
        {
            resolved = uri;
            return new ResolvedSpecifier(
                moduleRequest,
                resolved.AbsoluteUri,
                resolved,
                SpecifierType.RelativeOrAbsolute
            );
        }
        else
        {
            return new ResolvedSpecifier(
                moduleRequest,
                specifier,
                Uri: null,
                SpecifierType.Bare
            );
        }
    }

    private string ResolvePath(string specifier, string referencingModuleLocation)
    {
        if (string.IsNullOrEmpty(specifier)) { return null; }

        string resolvedPath = String.Empty;
        if (Path.IsPathRooted(specifier))
        {
            return specifier;
        }
        else if (!string.IsNullOrEmpty(referencingModuleLocation))
        {
            string resPath = UtmxResourceLoader.ResolvePath(specifier);
            if (Godot.FileAccess.FileExists(resPath))
            {
                string referencingDir = Path.GetDirectoryName(referencingModuleLocation);
                resolvedPath = Path.Combine(referencingDir, resPath);
                if (!resolvedPath.EndsWith(".js")) resolvedPath += ".js";
            }
            else
            {
                resolvedPath = specifier;
            }
        }
        else
        {
            string resPath = UtmxResourceLoader.ResolvePath(specifier);
            if (!string.IsNullOrEmpty(resPath))
            {
                resolvedPath = resPath;
            }
            else
            {
                resolvedPath = specifier;
            }
            if (!resolvedPath.EndsWith(".js")) resolvedPath += ".js";
        }
        return resolvedPath;
    }
}
