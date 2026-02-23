using Godot;
using System;

[GlobalClass]
public partial class GameCamera : Camera2D
{
    [Export(PropertyHint.Range, "0,20,0.1")]
    public Vector2 MaxShake = new(3f, 3f);

    [Export(PropertyHint.Range, "1,30,1")]
    public Vector2 ShakeFrequency = new(10f, 10f);

    [Export]
    public bool IsRandomX = true;

    [Export]
    public bool IsRandomY = true;

    [Export(PropertyHint.Range, "0.1,5,0.1")]
    public float DefaultShakeDuration = 0.1f;

    private float _shakeTotalDuration;
    private float _shakeRemainingDuration;
    private Vector2 _shakeTimer;
    private Vector2 _shakeInterval;
    private Vector2 _activeShakeAmplitude;
    private Vector2 _activeShakeFrequency;
    private Vector2 _shakeBaseOffset;
    private bool _shakePosX;
    private bool _shakePosY;
    private Vector2 _shakeOffset;
    private ulong _frequencyRestoreTicket = 0;

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
            float falloff = shakeRatio * shakeRatio; // smoother easing-out near the end

            UpdateShakeAxis(delta, isX: true);
            UpdateShakeAxis(delta, isX: false);

            Vector2 targetOffset = _shakeBaseOffset * falloff;
            float smoothWeight = Mathf.Clamp(delta * 35f, 0f, 1f);
            _shakeOffset = _shakeOffset.Lerp(targetOffset, smoothWeight);
            Offset = new Vector2(Mathf.Round(_shakeOffset.X), Mathf.Round(_shakeOffset.Y));
        }
        else if (_shakeOffset != Vector2.Zero)
        {
            ResetShakeState();
        }
    }

    private void UpdateShakeAxis(float delta, bool isX)
    {
        float amplitude = isX ? _activeShakeAmplitude.X : _activeShakeAmplitude.Y;
        if (Mathf.Abs(amplitude) <= 0.01f)
        {
            if (isX) _shakeBaseOffset.X = 0f;
            else _shakeBaseOffset.Y = 0f;
            return;
        }

        if (isX)
        {
            _shakeTimer.X += delta;
            while (_shakeTimer.X >= _shakeInterval.X)
            {
                _shakeTimer.X -= _shakeInterval.X;
                _shakeBaseOffset.X = CalculateShakeOffset(amplitude, ref _shakePosX, IsRandomX);
            }
        }
        else
        {
            _shakeTimer.Y += delta;
            while (_shakeTimer.Y >= _shakeInterval.Y)
            {
                _shakeTimer.Y -= _shakeInterval.Y;
                _shakeBaseOffset.Y = CalculateShakeOffset(amplitude, ref _shakePosY, IsRandomY);
            }
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
        _frequencyRestoreTicket++;
        _activeShakeFrequency = SanitizeFrequency(ShakeFrequency);
        _activeShakeAmplitude = shakeAmplitude == Vector2.Zero
            ? SanitizeAmplitude(MaxShake)
            : SanitizeAmplitude(shakeAmplitude);

        _shakeTotalDuration = ResolveShakeDuration(duration);
        _shakeRemainingDuration = _shakeTotalDuration;
        _shakeTimer = Vector2.Zero;
        _shakeBaseOffset = Vector2.Zero;
        _shakeOffset = Vector2.Zero;
        _shakePosX = false;
        _shakePosY = false;
        UpdateShakeIntervals();

        // Force one immediate sample so very short shakes are still visible.
        if (Mathf.Abs(_activeShakeAmplitude.X) > 0.01f)
            _shakeBaseOffset.X = CalculateShakeOffset(_activeShakeAmplitude.X, ref _shakePosX, IsRandomX);
        if (Mathf.Abs(_activeShakeAmplitude.Y) > 0.01f)
            _shakeBaseOffset.Y = CalculateShakeOffset(_activeShakeAmplitude.Y, ref _shakePosY, IsRandomY);
    }

    public void StartShake(float duration, Vector2 amplitude, Vector2 tempFrequency)
    {
        _frequencyRestoreTicket++;
        ulong ticket = _frequencyRestoreTicket;
        float resolvedDuration = ResolveShakeDuration(duration);

        _activeShakeFrequency = SanitizeFrequency(tempFrequency);
        _activeShakeAmplitude = amplitude == Vector2.Zero
            ? SanitizeAmplitude(MaxShake)
            : SanitizeAmplitude(amplitude);

        _shakeTotalDuration = resolvedDuration;
        _shakeRemainingDuration = resolvedDuration;
        _shakeTimer = Vector2.Zero;
        _shakeBaseOffset = Vector2.Zero;
        _shakeOffset = Vector2.Zero;
        _shakePosX = false;
        _shakePosY = false;
        UpdateShakeIntervals();

        if (Mathf.Abs(_activeShakeAmplitude.X) > 0.01f)
            _shakeBaseOffset.X = CalculateShakeOffset(_activeShakeAmplitude.X, ref _shakePosX, IsRandomX);
        if (Mathf.Abs(_activeShakeAmplitude.Y) > 0.01f)
            _shakeBaseOffset.Y = CalculateShakeOffset(_activeShakeAmplitude.Y, ref _shakePosY, IsRandomY);

        SceneTree tree = GetTree();
        if (tree == null) return;

        tree.CreateTimer(resolvedDuration).Timeout += () =>
        {
            if (!IsInsideTree() || ticket != _frequencyRestoreTicket) return;
            _activeShakeFrequency = SanitizeFrequency(ShakeFrequency);
            UpdateShakeIntervals();
        };
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
        _shakeBaseOffset = Vector2.Zero;
        _shakeOffset = Vector2.Zero;
        _activeShakeAmplitude = SanitizeAmplitude(MaxShake);
        _activeShakeFrequency = SanitizeFrequency(ShakeFrequency);
        _shakePosX = false;
        _shakePosY = false;
        Offset = Vector2.Zero;
    }

    private void UpdateShakeIntervals()
    {
        _shakeInterval.X = 1f / Mathf.Max(_activeShakeFrequency.X, 1f);
        _shakeInterval.Y = 1f / Mathf.Max(_activeShakeFrequency.Y, 1f);
    }

    private float ResolveShakeDuration(float duration)
    {
        return duration > 0.01f ? duration : DefaultShakeDuration;
    }

    private static Vector2 SanitizeAmplitude(Vector2 value)
    {
        return new Vector2(Mathf.Abs(value.X), Mathf.Abs(value.Y));
    }

    private static Vector2 SanitizeFrequency(Vector2 value)
    {
        return new Vector2(Mathf.Max(Mathf.Abs(value.X), 1f), Mathf.Max(Mathf.Abs(value.Y), 1f));
    }
}
