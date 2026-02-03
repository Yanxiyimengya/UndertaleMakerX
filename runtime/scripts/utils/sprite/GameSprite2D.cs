using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class GameSprite2D : AnimatedSprite2D 
{
	public const string DEFAULT_ANIM_NAME = "default";
	[Signal]
	public delegate void TextureChangedEventHandler();
	[Export]
	public string[] TexturesPath {
		get => _texturesPath;
		set
		{
			if (_texturesPath != value)
			{
				_texturesPath = value;
				if (_texturesPath.Length == 1)
				{
					SetTextures(_texturesPath[0]);
				}
				else
				{
					SetTextures(_texturesPath);
				}
			}
		}
	}
	[Export]
	public virtual Texture2D[] Textures
	{
		get => _textures;
		set
		{
			_textures = value;
			EmitSignal(SignalName.TextureChanged, []);
		}
	}
	protected Texture2D[] _textures;
	protected string[] _texturesPath;

	public GameSprite2D()
	{
		if (SpriteFrames == null)
			SpriteFrames = new SpriteFrames();
	}

	public void SetTextures(string texturePath)
	{
		Resource res = UtmxResourceLoader.Load(texturePath);
		if (res != null && res is Texture2D texture)
		{
			if (SpriteFrames.HasAnimation(DEFAULT_ANIM_NAME))
				SpriteFrames.Clear(DEFAULT_ANIM_NAME);
			SpriteFrames.AddFrame(DEFAULT_ANIM_NAME, texture);
			Textures = [texture];
		}
	}
	public void SetTextures(string[] texturesPath)
	{
		if (SpriteFrames.HasAnimation(DEFAULT_ANIM_NAME))
			SpriteFrames.Clear(DEFAULT_ANIM_NAME);

		var _texturesArray = new Texture2D[texturesPath.Length];
		int index = 0;
		foreach (string texturePath in texturesPath)
		{
			Resource res = UtmxResourceLoader.Load(texturePath);
			if (res != null && res is Texture2D texture)
			{
				SpriteFrames.AddFrame(DEFAULT_ANIM_NAME, texture);
				_texturesArray[index] = texture;
				index += 1;
			}
		}
		Textures = _texturesArray;
		Play(DEFAULT_ANIM_NAME);
	}
}
