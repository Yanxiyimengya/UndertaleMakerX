using System;
using System.Collections.Generic;
using Godot;
using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Modules;

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
		string code = file.GetAsText();
		file.Close();
		Module buildModule = ModuleFactory.BuildSourceTextModule(engine, Jint.Engine.PrepareModule(code, source));
		_moduleCache[source] = buildModule;

		return buildModule;
	}

	public ResolvedSpecifier Resolve(string referencingModuleLocation, ModuleRequest moduleRequest)
	{
		string specifier = moduleRequest.Specifier;
		string path = ResolvePath(referencingModuleLocation, specifier);

		Uri resolved;
		if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
		{
			resolved = uri;
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

		return new ResolvedSpecifier(
			moduleRequest,
			resolved.AbsoluteUri,
			resolved,
			SpecifierType.RelativeOrAbsolute
		);
	}

	private string ResolvePath(string referencingModuleLocation, string specifier)
	{
		string resolvedPath = UTMXResourceLoader.ResolvePath(specifier);
		if (! resolvedPath.EndsWith(".js"))
		{
			resolvedPath += ".js";
		}
		return resolvedPath;
	}
}
