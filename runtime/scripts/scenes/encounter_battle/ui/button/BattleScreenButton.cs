using Godot;
using System;

[GlobalClass]
public partial class BattleScreenButton : GameSprite2D
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
			Frame = value ? 1 : 0;
		}
	}

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
		Stop();
	}


    public override void SetTextures(string[] texturesPath)
    {
		base.SetTextures(texturesPath);
		Stop();
    }

    public void SetSoulPosition(Vector2 position)
    {
        soulMarker.Position = position;
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
