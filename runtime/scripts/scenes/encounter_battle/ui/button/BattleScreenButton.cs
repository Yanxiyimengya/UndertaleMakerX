using Godot;
using System;

[GlobalClass]
public partial class BattleScreenButton : Node2D
{
	[Signal]
	delegate void ButtonPressedEventHandler();

	[Export]
	public bool Hover
	{
		get => _hover;
		set 
		{
			_hover = value;
			if (buttonSprite != null)
			{
				buttonSprite.Texture = value ? buttonPressedTexture : buttonTexture;
			}
		}
	}
	[Export]
	public Texture2D buttonTexture;
	[Export]
	public Texture2D buttonPressedTexture;

	[Export]
	public Sprite2D buttonSprite;

	[Export]
	public Marker2D soulMarker;

	public string ButtonFocusNeighborLeftId = "";
	public string ButtonFocusNeighborRightId = "";
	public string ButtonFocusNeighborUpId = "";
	public string ButtonFocusNeighborDownId = "";

	private bool _hover;

	public override void _Ready()
	{
		Hover = false;
	}

	public Transform2D GetSoulTransform()
	{
		return soulMarker.GlobalTransform;
	}

	public virtual void PressButton()
	{
		this.EmitSignal(BattleScreenButton.SignalName.ButtonPressed, []);
	}

}
