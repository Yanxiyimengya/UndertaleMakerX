using Godot;
using System;
using System.Collections.Generic;
using System.IO;

internal static class UtmxResourceLoader
{
	private static readonly HashSet<string> TextureExts = new(StringComparer.OrdinalIgnoreCase)
	{
		".png", ".jpg", ".jpeg", ".webp", ".bmp", ".tga", ".svg", ".hdr", ".exr"
	};

	private static readonly HashSet<string> AudioExts = new(StringComparer.OrdinalIgnoreCase)
	{
		".ogg", ".oga", ".mp3", ".wav"
	};

	private static readonly HashSet<string> FontExts = new(StringComparer.OrdinalIgnoreCase)
	{
		".ttf", ".otf", ".ttc", ".woff", ".woff2"
	};

	private static readonly HashSet<string> BitmapFontExts = new(StringComparer.OrdinalIgnoreCase)
	{
		".fnt"
	};

	private static readonly HashSet<string> ShaderExts = new(StringComparer.OrdinalIgnoreCase)
	{
		".gdshader", ".shader"
	};

	private static readonly HashSet<string> ShaderIncludeExts = new(StringComparer.OrdinalIgnoreCase)
	{
		".gdshaderinc"
	};

	private static readonly HashSet<string> VideoExts = new(StringComparer.OrdinalIgnoreCase)
	{
		".ogv"
	};

	public static System.Collections.Generic.Dictionary<string, Resource> resourceCache = new();
	public static Resource Load(string resPath)
	{
		string resNewPath = ResolvePath(resPath);
		if (string.IsNullOrEmpty(resNewPath)) return null;
		if (resourceCache.TryGetValue(resNewPath, out Resource res) && res != null)
			return res;
		Resource utmxRes = LoadByExtensionFallback(resNewPath);
		if (utmxRes != null) 
			resourceCache.Add(resNewPath, utmxRes);
		return utmxRes;
	}

	private static Resource LoadByExtensionFallback(string path)
	{
		if (!Godot.FileAccess.FileExists(path))
			return null;

		string ext = Path.GetExtension(path);
		if (string.IsNullOrEmpty(ext))
			return null;
		if (TextureExts.Contains(ext))
			return LoadTexture(path);
		if (AudioExts.Contains(ext))
			return LoadAudio(path, ext);
		if (FontExts.Contains(ext))
			return LoadFont(path);
		if (BitmapFontExts.Contains(ext))
			return LoadBitmapFont(path);
		if (ShaderExts.Contains(ext))
			return LoadShader(path);
		if (ShaderIncludeExts.Contains(ext))
			return LoadShaderInclude(path);
		if (VideoExts.Contains(ext))
			return LoadVideo(path);
		return null;
	}

	private static Texture2D LoadTexture(string path)
	{
		Image image = Image.LoadFromFile(path);
		if (image == null || image.IsEmpty())
			return null;
		return ImageTexture.CreateFromImage(image);
	}

	private static AudioStream LoadAudio(string path, string ext)
	{
		return ext.ToLowerInvariant() switch
		{
			".ogg" => AudioStreamOggVorbis.LoadFromFile(path),
			".oga" => AudioStreamOggVorbis.LoadFromFile(path),
			".mp3" => AudioStreamMP3.LoadFromFile(path),
			".wav" => AudioStreamWav.LoadFromFile(path),
			_ => null
		};
	}

	private static Font LoadFont(string path)
	{
		FontFile font = new();
		Error err = font.LoadDynamicFont(path);
		font.Antialiasing = TextServer.FontAntialiasing.None;
		if (err != Error.Ok)
		{
			font.Dispose();
			return null;
		}
		return font;
	}

	private static Font LoadBitmapFont(string path)
	{
		FontFile font = new();
		Error err = font.LoadBitmapFont(path);
		font.Antialiasing = TextServer.FontAntialiasing.None;
		if (err != Error.Ok)
		{
			font.Dispose();
			return null;
		}
		return font;
	}

	private static Shader LoadShader(string path)
	{
		string shaderCode = LoadTextFile(path);
		if (string.IsNullOrEmpty(shaderCode))
			return null;
		Shader shader = new();
		shader.SetPathCache(path);
		shader.Code = shaderCode;
		return shader;
	}

	private static ShaderInclude LoadShaderInclude(string path)
	{
		string shaderIncludeCode = LoadTextFile(path);
		if (string.IsNullOrEmpty(shaderIncludeCode))
			return null;
		ShaderInclude shaderInclude = new();
		shaderInclude.SetPathCache(path);
		shaderInclude.Code = shaderIncludeCode;
		return shaderInclude;
	}

	private static VideoStream LoadVideo(string path)
	{
		return new VideoStreamTheora { File = path };
	}

	private static string LoadTextFile(string path)
	{
		Godot.FileAccess file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
		if (file == null)
			return null;
		string text = file.GetAsText();
		file.Dispose();
		return text;
	}

	public static Godot.FileAccess OpenFile(string filePath, Godot.FileAccess.ModeFlags flags)
	{
		if (string.IsNullOrEmpty(filePath)) return null;
		filePath = ResolvePath(filePath);
		if (Godot.FileAccess.FileExists(filePath))
		{
			return Godot.FileAccess.Open(filePath, flags);
		}
		return null;
	}

	public static string ResolvePath(string path)
	{
		if (string.IsNullOrEmpty(path)) return "";
		if (Path.IsPathRooted(path) || path.StartsWith("res://") || path.StartsWith("uid://"))
		{
			return path;
		}
		string resPackPath = $"res://{EngineProperties.DATAPACK_RESOURCE_PATH}/{path}";
		return resPackPath;
	}

	public static void ClearCache()
	{
		resourceCache.Clear();

	}
}
