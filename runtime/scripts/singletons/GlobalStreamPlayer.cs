using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalStreamPlayer : UTMXSingleton<GlobalStreamPlayer>
{
	private AudioStreamPlayer soundPlayer = null;
	private Dictionary<string, AudioStreamPlayer> bgmPlayers = new Dictionary<string, AudioStreamPlayer>();
	private Queue<AudioStreamPlayer> bgmPlayersPool = new Queue<AudioStreamPlayer>();

	public GlobalStreamPlayer()
	{
		soundPlayer = new AudioStreamPlayer();
		soundPlayer.Stream = new AudioStreamPolyphonic();
	}
	public override void _EnterTree()
	{
		base._EnterTree();
		AddChild(soundPlayer);

    }

	public void PlaySound(AudioStream stream)
	{
		if (!soundPlayer.Playing) soundPlayer.Play();
        if (soundPlayer.GetStreamPlayback() is AudioStreamPlaybackPolyphonic playback)
		{
			playback.PlayStream(stream);
		}
	}

	public void PlayBGM(string bgmId, AudioStream stream, bool loop = false)
	{
		AudioStreamPlayer soundPlayer;
		if (bgmPlayersPool.Count > 0)
		{
			soundPlayer = bgmPlayersPool.Dequeue();
			soundPlayer.ProcessMode = Node.ProcessModeEnum.Inherit;
		}
		else
		{
			soundPlayer = new AudioStreamPlayer();
			soundPlayer.Connect(AudioStreamPlayer.SignalName.Finished, Callable.From(
				() =>
				{
					if (soundPlayer.GetMeta("loop").AsBool())
					{
						soundPlayer.Play();
					}
					else
					{
						StopBGM(soundPlayer.GetMeta("bgmId").AsString());
					}
				}
			));
			soundPlayer.SetMeta("id", bgmId);
			soundPlayer.SetMeta("loop", loop);
			bgmPlayers.Add(bgmId, soundPlayer);
		}
		soundPlayer.Stream = stream;
		soundPlayer.Play();
	}

	public void StopBGM(string id)
	{
		if (bgmPlayers.TryGetValue(id, out AudioStreamPlayer player))
		{
			bgmPlayers.Remove(id);
			player.Stop();
			player.ProcessMode = Node.ProcessModeEnum.Disabled;
			bgmPlayersPool.Enqueue(player);
		}
	}

	public void SetBgmVolume(string id, float targetVolumeDB, bool smooth = false, float duration = 1.0F)
	{
		if (bgmPlayers.TryGetValue(id, out AudioStreamPlayer player))
		{
			if (smooth)
			{
				Tween t = GetTree().CreateTween();
				t.TweenProperty(player, "volume_db", targetVolumeDB, duration);
			}
			else
			{
				player.VolumeDb = targetVolumeDB;
			}
		}
	}

}
