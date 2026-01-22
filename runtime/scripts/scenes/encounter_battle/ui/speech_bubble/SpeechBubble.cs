using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

[GlobalClass]
public partial class SpeechBubble : Node2D
{
	[Export]
	public string Text {
		get => _text;
		set
		{
			_text = value;
			SpeechBubbleTextTyper.Start(value);
		}
	}
	[Export]
	public Vector2 Size = new Vector2(50F, 50F);
	[Export(PropertyHint.Enum, "Top,Bottom,Left,Right")]
	public int Dir = 2;
	[Export]
	public float SpikeOffset = 0F;
	[Export]
	public bool HideSpike = false;
	[Export]
	public bool InSpike = true;
	[Export]
	public TextTyper SpeechBubbleTextTyper;
	[Export]
	public TextureRect SpeechBubbleSpikeTexture;

	private Vector2 _minimumSize = new Vector2(45, 45);
	private string _text;

	public override void _Ready()
	{
		SpeechBubbleTextTyper.Start(Text);
	}

	public override void _Process(double delta)
	{
		Size = new Vector2(MathF.Max(Size.X, _minimumSize.X), MathF.Max(Size.Y, _minimumSize.Y));
		SpeechBubbleTextTyper.Size = Size;
		SpeechBubbleSpikeTexture.Visible = !HideSpike;

		if (!HideSpike) { 
			switch (Dir)
			{
				case 0:
					SpeechBubbleSpikeTexture.RotationDegrees = 270.0F;
					SpikeOffset = Mathf.Clamp(SpikeOffset, 0, Size.X - SpeechBubbleSpikeTexture.Size.X - 15F);
					break;

				case 1:
					SpeechBubbleSpikeTexture.RotationDegrees = 90.0F;
					SpikeOffset = Mathf.Clamp(SpikeOffset, 0, Size.X - SpeechBubbleSpikeTexture.Size.X - 15F);
					break;

				case 2:
					SpeechBubbleSpikeTexture.RotationDegrees = 0.0F;
					SpikeOffset = Mathf.Clamp(SpikeOffset, 0, Size.Y - SpeechBubbleSpikeTexture.Size.Y - 18F);
					break;

				case 3:
					SpeechBubbleSpikeTexture.RotationDegrees = 180.0F;
					SpikeOffset = Mathf.Clamp(SpikeOffset, 0, Size.Y - SpeechBubbleSpikeTexture.Size.Y - 18F);
					break;
			}
		}

		if (InSpike)
		{
			switch (Dir)
			{
				case 0:
					SpeechBubbleSpikeTexture.Position = new Vector2(SpeechBubbleSpikeTexture.Size.Y * 0.5F
						- 18F,
						0F);
					SpeechBubbleTextTyper.Position = new Vector2(-SpikeOffset - 18F,
						-SpeechBubbleSpikeTexture.Size.X - SpeechBubbleTextTyper.Size.Y);
					break;

				case 1:
					SpeechBubbleSpikeTexture.Position = new Vector2(SpeechBubbleSpikeTexture.Size.Y * 0.5F,
						0F);
					SpeechBubbleTextTyper.Position = new Vector2(-SpikeOffset - 18F,
						SpeechBubbleSpikeTexture.Size.X);
					break;

				case 2:
					SpeechBubbleSpikeTexture.Position = new Vector2(0.0F, 
						-SpeechBubbleSpikeTexture.Size.Y * 0.5F);
					SpeechBubbleTextTyper.Position = new Vector2(SpeechBubbleSpikeTexture.Size.X,
						-SpeechBubbleSpikeTexture.Size.Y * 0.5F - SpikeOffset - 10F);
					break;

				case 3:
					SpeechBubbleSpikeTexture.Position = new Vector2(0.0F,
						SpeechBubbleSpikeTexture.Size.Y * 0.5F);
					SpeechBubbleTextTyper.Position = new Vector2(
						-SpeechBubbleSpikeTexture.Size.X - SpeechBubbleTextTyper.Size.X
						,
						-SpeechBubbleSpikeTexture.Size.Y * 0.5F - SpikeOffset - 10F);
					break;
			}
		}
		else
		{
			SpeechBubbleTextTyper.Position = -Size * 0.5F;
			switch (Dir)
			{
				case 0:
					SpeechBubbleSpikeTexture.Position = new Vector2(
						-Size.X * 0.5F + 10 + SpikeOffset,
						SpeechBubbleSpikeTexture.Size.X + Size.Y * 0.5F);
					break;

				case 1:
					SpeechBubbleSpikeTexture.Position = new Vector2(
						-Size.X * 0.5F + SpeechBubbleSpikeTexture.Size.X * 0.5F + 15 + SpikeOffset,
						-SpeechBubbleSpikeTexture.Size.X - Size.Y * 0.5F);
					break;

				case 2:
					SpeechBubbleSpikeTexture.Position = SpeechBubbleTextTyper.Position +
						new Vector2(-SpeechBubbleSpikeTexture.Size.X, 
						SpeechBubbleSpikeTexture.Size.Y * 0.5F + SpikeOffset);
					break;

				case 3:
					SpeechBubbleSpikeTexture.Position = new Vector2(Size.X * 0.5F + SpeechBubbleSpikeTexture.Size.X,
						-Size.Y * 0.5F + SpeechBubbleSpikeTexture.Size.Y + 10 + SpikeOffset);
					break;
			}
		}
	}
}
