using System;
using System.Collections.Generic;
using System.IO;
using Godot;

[GlobalClass]
public partial class UtmxGlobalStreamPlayer : Node
{
	[Export]
	public AudioStream SelectSoundStream;
	[Export]
	public AudioStream SqueakSoundStream;
	[Export]
	public AudioStream EscapedSoundStream;
	[Export]
	public AudioStream TextTyperSoundStream;
	[Export]
	public AudioStream EnemyDialogueSoundStream;
	[Export]
	public AudioStream HurtSoundStream;
	[Export]
	public AudioStream HeartBeatBreakSoundStream;
	[Export]
	public AudioStream HeartPlosionSoundStream;

	[Export]
	public AudioStream GameOverMusicSoundStream;

	public static UtmxGlobalStreamPlayer Instance;
	private AudioStreamPlayer soundPlayer = null;
	private Dictionary<string, AudioStreamPlayer> bgmPlayers = new Dictionary<string, AudioStreamPlayer>();
	private Queue<AudioStreamPlayer> bgmPlayersPool = new Queue<AudioStreamPlayer>();
	private Dictionary<string, AudioStream> _streamLibrary = new Dictionary<string, AudioStream>();

	public UtmxGlobalStreamPlayer()
	{
		soundPlayer = new AudioStreamPlayer();
		soundPlayer.Stream = new AudioStreamPolyphonic();
	}
	public override void _EnterTree()
	{
		Instance = this;
		AddChild(soundPlayer);

		AppendStreamToLibrary("SELECT", SelectSoundStream);
		AppendStreamToLibrary("SQUEAK", SqueakSoundStream);
		AppendStreamToLibrary("ESCAPED", EscapedSoundStream);
		AppendStreamToLibrary("TEXT_TYPER_VOICE", TextTyperSoundStream);
		AppendStreamToLibrary("ENEMY_VOICE", EnemyDialogueSoundStream);
		AppendStreamToLibrary("HURT", HurtSoundStream);

		AppendStreamToLibrary("HEART_BEAT_BREAK", HeartBeatBreakSoundStream);
		AppendStreamToLibrary("HEART_PLOSION", HeartPlosionSoundStream);

		AppendStreamToLibrary("GAME_OVER", GameOverMusicSoundStream);
	}

	public long PlaySoundFromStream(AudioStream stream)
	{
		if (!soundPlayer.Playing) soundPlayer.Play();
		if (soundPlayer.GetStreamPlayback() is AudioStreamPlaybackPolyphonic playback)
		{
			return playback.PlayStream(stream);
		}
		return -1;
	}

	public long PlaySoundFromPath(string path)
	{
		Resource res = UtmxResourceLoader.Load(path);
		if (res != null && res is AudioStream stream)
		{
			return PlaySoundFromStream(stream);
		}
		return -1;
	}

	public void StopSound(long id)
	{
		if (soundPlayer.Playing)
		{
			if (soundPlayer.GetStreamPlayback() is AudioStreamPlaybackPolyphonic playback)
			{
				playback.StopStream(id);
			}
		}
		throw new ArgumentException($"Sound with id '{id}' not found.");
	}


	public void PlayBgmFormStream(string bgmId, AudioStream stream, bool loop = false)
	{
		if (string.IsNullOrEmpty(bgmId)) return;
		AudioStreamPlayer player;
		if (bgmPlayers.TryGetValue(bgmId, out player))
		{
			player.Stop();
			if (player.HasMeta("volume_tween"))
			{
				Tween _tween = (Tween)player.GetMeta("volume_tween");
				if (_tween != null && _tween.IsValid()) _tween.Kill();
			}
			if (player.HasMeta("pitch_tween"))
			{
				Tween _tween = (Tween)player.GetMeta("pitch_tween");
				if (_tween != null && _tween.IsValid()) _tween.Kill();
			}

			player.VolumeDb = 0.0F;
			player.PitchScale = 1.0F;
		}
		else if (bgmPlayersPool.Count > 0)
		{
			player = bgmPlayersPool.Dequeue();
			player.ProcessMode = Node.ProcessModeEnum.Inherit;
			bgmPlayers.Add(bgmId, player);
		}
		else
		{
			player = new AudioStreamPlayer();
			player.Finished += () =>
			{
				bool _isLoop = player.GetMeta("loop").AsBool();
				if (_isLoop)
				{
					player.Play();
				}
				else
				{
					StopBgm(bgmId);
				}
			};
			bgmPlayers.Add(bgmId, player);
			this.AddChild(player);
		}
		player.Stream = stream;
		player.Play();
		player.SetMeta("loop", loop);
	}

	public void PlayBgmFromPath(string bgmId, string path, bool loop = false)
	{
		if (string.IsNullOrEmpty(bgmId)) return;
		Resource res = UtmxResourceLoader.Load(path);
		if (res != null && res is AudioStream stream)
		{
			PlayBgmFormStream(bgmId, stream, loop);
		}
	}
	public void StopBgm(string bgmId)
	{
		if (string.IsNullOrEmpty(bgmId)) return;
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			bgmPlayers.Remove(bgmId);
			bgmPlayersPool.Enqueue(player);
			player.ProcessMode = ProcessModeEnum.Disabled;
			player.Stop();
			return;
		}
		throw new ArgumentException($"Bgm player with id '{bgmId}' not found.");
	}
	public void StopAll()
	{
		soundPlayer.Stop();
		foreach (string playerId in bgmPlayers.Keys)
		{
			StopBgm(playerId);
		}
	}

	public void SetBgmPaused(string bgmId, bool paused)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			player.StreamPaused = paused;
			return;
		}
		throw new ArgumentException($"Bgm player with id '{bgmId}' not found.");
	}
	public bool GetBgmPaused(string bgmId)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			return player.StreamPaused;
		}
		throw new ArgumentException($"Bgm player with id '{bgmId}' not found.");
	}

	public float GetBgmVolume(string bgmId)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			return player.VolumeDb;
		}
		throw new ArgumentException($"Bgm player with id '{bgmId}' not found.");
	}

	public void SetBgmVolume(string bgmId, float volumeDb, float duration = 0)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			if (player.HasMeta("volume_tween"))
			{
				Tween _tween = (Tween)player.GetMeta("volume_tween");
				if (_tween != null && _tween.IsValid()) _tween.Kill();
			}
			if (duration > 0.0F)
			{
				Tween _tween = player.CreateTween();
				_tween.TweenProperty(player, "volume_db", volumeDb, duration);
				player.SetMeta("volume_tween", _tween);
			}
			else
			{
				player.VolumeDb = volumeDb;
			}
			return;
		}
		throw new ArgumentException($"Bgm player with id '{bgmId}' not found.");
	}
	public float GetBgmPitch(string bgmId)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			return player.PitchScale;
		}
		throw new ArgumentException($"Bgm player with id '{bgmId}' not found.");
	}

	public void SetBgmPitch(string bgmId, float pitch, float duration = 0)
	{
		if (pitch < 0)
		{
			pitch = 0.01F;
		}
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			if (player.HasMeta("pitch_tween"))
			{
				Tween _tween = (Tween)player.GetMeta("pitch_tween");
				if (_tween != null && _tween.IsValid()) _tween.Kill();
			}
			if (duration > 0)
			{
				Tween _tween = player.CreateTween();
				_tween.TweenProperty(player, "pitch_scale", pitch, duration);
				player.SetMeta("pitch_tween", _tween);
			}
			else
			{
				player.PitchScale = pitch;
			}
			return;
		}
		throw new ArgumentException($"Bgm player with id '{bgmId}' not found.");
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

	public AudioStream GetStreamFormLibrary(string id)
	{
		return _streamLibrary[id];
	}
}
