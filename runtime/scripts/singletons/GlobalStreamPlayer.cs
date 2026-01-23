using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class GlobalStreamPlayer : Node
{
	[Export]
	public AudioStream SelectSoundStream;
	[Export]
	public AudioStream SqueakSoundStream;
	[Export]
	public AudioStream EscapedSoundStream;
	[Export]
	public AudioStream TextSoundStream;
	[Export]
	public AudioStream EnemyDialogueSoundStream;

	public static GlobalStreamPlayer Instance;
	private AudioStreamPlayer soundPlayer = null;
	private Dictionary<string, AudioStreamPlayer> bgmPlayers = new Dictionary<string, AudioStreamPlayer>();
	private Queue<AudioStreamPlayer> bgmPlayersPool = new Queue<AudioStreamPlayer>();
	private Dictionary<string, AudioStream> _streamLibrary = new Dictionary<string, AudioStream>();

	public GlobalStreamPlayer()
	{
		soundPlayer = new AudioStreamPlayer();
		soundPlayer.Stream = new AudioStreamPolyphonic();
	}
	public override void _EnterTree()
	{
		Instance = this;
		base._EnterTree();
		AddChild(soundPlayer);
		
		AppendStreamToLibrary("SELECT", SelectSoundStream);
		AppendStreamToLibrary("SQUEAK", SqueakSoundStream);
		AppendStreamToLibrary("ESCAPED", EscapedSoundStream);
		AppendStreamToLibrary("TEXT_TYPER_VOICE", TextSoundStream);
		AppendStreamToLibrary("ENEMY_VOICE", EnemyDialogueSoundStream);
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

	public void AppendStreamToLibrary(string id, AudioStream stream)
	{
		if (stream == null) return;
		if (!_streamLibrary.TryGetValue(id, out _))
		{
			_streamLibrary[id] = stream; 
		}
	}

	public void RemoveStreamToLibrary(string id)
	{
		if (_streamLibrary.TryGetValue(id, out _))
		{
			_streamLibrary.Remove(id);
		}
	}

	public AudioStream GetStream(string id)
	{
		return _streamLibrary[id];
	}
}
