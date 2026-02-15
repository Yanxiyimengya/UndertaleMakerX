using Godot;
using System;

[GlobalClass]
public partial class BattleCamera : Camera2D
{
	[Export(PropertyHint.Range, "0,20,0.1")]
	public Vector2 MaxShake = new(3f, 3f);

	[Export(PropertyHint.Range, "1,30,1")]
	public Vector2 ShakeFrequency = new(10f, 10f);

	[Export]
	public bool IsRandomX = false;

	[Export]
	public bool IsRandomY = false;

	[Export(PropertyHint.Range, "0.1,5,0.1")]
	public float DefaultShakeDuration = 0.1f;

	private float _shakeTotalDuration;
	private float _shakeRemainingDuration;
	private Vector2 _shakeTimer;
	private Vector2 _shakeInterval;
	private bool _shakePosX;
	private bool _shakePosY;
	private Vector2 _shakeOffset;

	public override void _Ready()
	{
		ResetShakeState();
		UpdateShakeIntervals();
		IgnoreRotation = false;
	}

	public override void _Process(double delta)
	{
		UpdateCameraShake((float)delta);
	}

	private void UpdateCameraShake(float delta)
	{
		if (_shakeRemainingDuration > 0f)
		{
			_shakeRemainingDuration = Mathf.Max(_shakeRemainingDuration - delta, 0f);
			float shakeRatio = _shakeTotalDuration > 0f ? _shakeRemainingDuration / _shakeTotalDuration : 0f;

			if (Mathf.Abs(MaxShake.X) > 0.01f)
			{
				_shakeTimer.X += delta;
				if (_shakeTimer.X >= _shakeInterval.X)
				{
					_shakeTimer.X = 0f;
					_shakeOffset.X = CalculateShakeOffset(MaxShake.X, ref _shakePosX, IsRandomX) * shakeRatio;
				}
			}
			else
			{
				_shakeOffset.X = 0f;
			}

			if (Mathf.Abs(MaxShake.Y) > 0.01f)
			{
				_shakeTimer.Y += delta;
				if (_shakeTimer.Y >= _shakeInterval.Y)
				{
					_shakeTimer.Y = 0f;
					_shakeOffset.Y = CalculateShakeOffset(MaxShake.Y, ref _shakePosY, IsRandomY) * shakeRatio;
				}
			}
			else
			{
				_shakeOffset.Y = 0f;
			}

			_shakeOffset = new Vector2(Mathf.Round(_shakeOffset.X), Mathf.Round(_shakeOffset.Y));
			Offset = _shakeOffset;
		}
		else if (_shakeOffset != Vector2.Zero)
		{
			ResetShakeState();
		}
	}

	private float CalculateShakeOffset(float maxAmplitude, ref bool flipState, bool isRandom)
	{
		if (isRandom)
		{
			return (float)GD.RandRange(-maxAmplitude, maxAmplitude);
		}
		else
		{
			flipState = !flipState;
			return flipState ? maxAmplitude : -maxAmplitude;
		}
	}

	public void StartShake(float duration = 0f, Vector2 shakeAmplitude = default)
	{
		_shakeTotalDuration = duration > 0.01f ? duration : DefaultShakeDuration;
		_shakeRemainingDuration = _shakeTotalDuration;

		if (shakeAmplitude != Vector2.Zero)
		{
			MaxShake = shakeAmplitude;
		}

		_shakeTimer = Vector2.Zero;
		UpdateShakeIntervals();
	}

	public void StartShake(float duration, Vector2 amplitude, Vector2 tempFrequency)
	{
		var oldFreq = ShakeFrequency;
		ShakeFrequency = tempFrequency;
		UpdateShakeIntervals();
		StartShake(duration, amplitude);
		GetTree().CreateTimer(duration).Timeout += () => { ShakeFrequency = oldFreq; UpdateShakeIntervals(); };
	}

	public void StopShake()
	{
		ResetShakeState();
	}

	private void ResetShakeState()
	{
		_shakeTotalDuration = 0f;
		_shakeRemainingDuration = 0f;
		_shakeTimer = Vector2.Zero;
		_shakeOffset = Vector2.Zero;
		_shakePosX = false;
		_shakePosY = false;
		Offset = Vector2.Zero;
	}

	private void UpdateShakeIntervals()
	{
		_shakeInterval.X = 1f / Mathf.Max(ShakeFrequency.X, 1f);
		_shakeInterval.Y = 1f / Mathf.Max(ShakeFrequency.Y, 1f);
	}
}
