using Godot;
using System;

[Tool]
public partial class BattleScreenButton : Node2D
{
	[Export]
	public bool Pressed
	{
		get => pressed;
		set 
		{
			pressed = value;
			if (buttonSprite != null)
			{
				buttonSprite.Texture = value ? buttonPressedTexture : buttonTexture;
			}
		}
	}
	private bool pressed;
	[Export]
	public Texture2D buttonTexture;
	[Export]
	public Texture2D buttonPressedTexture;

	[Export]
	public Sprite2D buttonSprite;

	[Export]
	public Marker2D soulMarker;


	public override void _Ready()
	{
		Pressed = false;
	}

	public Vector2 GetSoulPosition()
	{
		return soulMarker.GlobalPosition;
	}

}
