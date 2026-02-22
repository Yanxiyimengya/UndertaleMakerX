using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class GameSprite2D : AnimatedSprite2D, IObjectPoolObject
{
	public const string DEFAULT_ANIM_NAME = "default";
	[Signal]
	public delegate void TextureChangedEventHandler();

	[Export]
	public bool Loop
	{
		get => SpriteFrames.GetAnimationLoop(DEFAULT_ANIM_NAME);
		set
		{
			SpriteFrames.SetAnimationLoop(DEFAULT_ANIM_NAME, value);
		}
	}
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
			if (_textures != value)
			{
				_textures = value;
				EmitSignal(SignalName.TextureChanged, []);
			}
		}
	}

	public GameShader ShaderInstance
	{
		get => _shaderInstance;
		set
		{
			_shaderInstance = value;
			Material = value.GetShaderMaterial();
		}
	}

	protected Texture2D[] _textures;
	protected string[] _texturesPath;
	protected GameShader _shaderInstance;

	public GameSprite2D()
	{
		if (SpriteFrames == null)
			SpriteFrames = new SpriteFrames();
	}
	public virtual void SetTextures(string[] texturesPath)
	{
		if (SpriteFrames == null) SpriteFrames = new();
		if (SpriteFrames.HasAnimation(DEFAULT_ANIM_NAME))
			SpriteFrames.Clear(DEFAULT_ANIM_NAME);
		if (texturesPath.Length > 0)
		{
			var _texturesArray = new Texture2D[texturesPath.Length];
			int index = 0;
			foreach (string texturePath in texturesPath)
			{
				Resource res = UtmxResourceLoader.Load(texturePath);
				if (res is Texture2D texture)
				{
					SpriteFrames.AddFrame(DEFAULT_ANIM_NAME, texture);
					_texturesArray[index] = texture;
				}
				index += 1;
			}
			Textures = _texturesArray;
			SetAnimation(DEFAULT_ANIM_NAME);
			Stop();
			if (_texturesArray.Length > 1)
			{
				PlayAnimation();
			}
		}
	}

	public void PlayAnimation()
	{
		if (SpriteFrames == null) return;
		Play(DEFAULT_ANIM_NAME);
	}
	public void Resume()
	{
		if (SpriteFrames == null) return;
		Play();
	}

	public virtual void Awake()
	{
		Transform = Transform2D.Identity;
		Modulate = Colors.White;
		Material = null;
		Offset = Vector2.Zero;
		TexturesPath = [];
		Textures = [];
        Loop = true;
	}

	public virtual void Disabled()
	{
	}

	public virtual void Destroy()
	{
		UtmxSceneManager.DeleteSprite(this);
	}
}
