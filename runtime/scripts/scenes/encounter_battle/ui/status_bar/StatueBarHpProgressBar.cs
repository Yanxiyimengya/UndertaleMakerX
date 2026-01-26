using Godot;
using System;

public partial class StatueBarHpProgressBar : PanelContainer
{
	[Export]
	public double Value {
		get => _value;
		set
		{
			_value = value;
			_UpdateSize();
		}
	}
	[Export]
	public double MaxValue {
		get => _maxValue;
		set {
			_maxValue = value;
			_UpdateSize();

		}
	}
	[Export]
	public Panel _backGround;
	[Export]
	public Panel _foreGround;

	private double _value = 20.0;
	private double _maxValue = 20.0;

	private void _UpdateSize()
	{
		_foreGround.Size = Vector2.Zero;
		_backGround.CustomMinimumSize = new Vector2(
			(float)Math.Round(_maxValue * 1.2F) + 1.0F, _backGround.CustomMinimumSize.Y);
		_foreGround.CustomMinimumSize = new Vector2(
			(float)Math.Round(_value * 1.2F) + 1.0F, _foreGround.CustomMinimumSize.Y);
	}
	

}
