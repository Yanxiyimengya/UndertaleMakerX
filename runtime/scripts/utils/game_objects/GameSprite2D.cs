using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class GameSprite2D : AnimatedSprite2D, IObjectPoolObject
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
				SetTextures(_texturesPath);
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

	public void SetTextures(string[] texturesPath)
	{
		if (SpriteFrames == null) SpriteFrames = new();
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
			}
			index += 1;
		}
        SetAnimation(DEFAULT_ANIM_NAME);
		Textures = _texturesArray;
		if (_texturesArray.Length > 1)
		{
			Play(DEFAULT_ANIM_NAME);
		}
    }
    public virtual void SetLoop(bool loop)
    {
		SpriteFrames.SetAnimationLoop(DEFAULT_ANIM_NAME, loop);
    }
    public virtual void Awake()
    {
		Transform = Transform2D.Identity;
		Modulate = Colors.White;
		Material = null;
        Offset = Vector2.Zero;
    }

    public virtual void Disabled()
    {
    }

    public virtual void Destroy()
    {
		UtmxSceneManager.DeleteSprite(this);
    }
}
