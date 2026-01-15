using Godot;
using System;

public partial class GlobalSoundPlayer : AudioStreamPlayer
{
	public static GlobalSoundPlayer Instance { get; private set; }

	public override void _Ready()
	{
		Stream = new AudioStreamPolyphonic();
		Instance = this;
	}

	public void PlaySound(AudioStream stream)
	{
		if (!Playing) this.Play();

		if (this.GetStreamPlayback() is AudioStreamPlaybackPolyphonic playback)
		{
			playback.PlayStream(stream);
		}
	}
}
