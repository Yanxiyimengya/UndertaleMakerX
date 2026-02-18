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
	public AudioStream HealSoundStream;
	[Export]
	public AudioStream LazSoundStream;
	[Export]
	public AudioStream HeartBeatBreakSoundStream;
	[Export]
	public AudioStream HeartPlosionSoundStream;
	[Export]
	public AudioStream GameOverMusicSoundStream;

	public static UtmxGlobalStreamPlayer Instance;
	private static AudioStreamPlayer soundPlayer = null;
	private static Dictionary<string, AudioStreamPlayer> bgmPlayers = new Dictionary<string, AudioStreamPlayer>();
	private static Queue<AudioStreamPlayer> bgmPlayersPool = new Queue<AudioStreamPlayer>();
	private static Dictionary<string, AudioStream> _streamLibrary = new Dictionary<string, AudioStream>();

	static UtmxGlobalStreamPlayer()
	{
		soundPlayer = new AudioStreamPlayer();
		soundPlayer.Bus = "Sound";
        soundPlayer.Stream = new AudioStreamPolyphonic();
	}
	public override void _EnterTree()
	{
		if (Instance != null && Instance != this)
		{
			QueueFree();
			return;
		}
		Instance = this;
		AddChild(soundPlayer);

		AppendStreamToLibrary("SELECT", SelectSoundStream);
		AppendStreamToLibrary("SQUEAK", SqueakSoundStream);
		AppendStreamToLibrary("ESCAPED", EscapedSoundStream);
		AppendStreamToLibrary("TEXT_TYPER_VOICE", TextTyperSoundStream);
		AppendStreamToLibrary("ENEMY_VOICE", EnemyDialogueSoundStream);
		AppendStreamToLibrary("HURT", HurtSoundStream);
		AppendStreamToLibrary("HEAL", HealSoundStream);
		AppendStreamToLibrary("LAZ", LazSoundStream);
		AppendStreamToLibrary("HEART_BEAT_BREAK", HeartBeatBreakSoundStream);
		AppendStreamToLibrary("HEART_PLOSION", HeartPlosionSoundStream);
		AppendStreamToLibrary("GAME_OVER", GameOverMusicSoundStream);
	}

	public override void _ExitTree()
	{
		Instance = null;
		soundPlayer = null;
		bgmPlayers.Clear();
		_streamLibrary.Clear();
		foreach (AudioStreamPlayer player in bgmPlayersPool)
		{
			player.QueueFree();
		}
		bgmPlayersPool.Clear();
	}

	public static long PlaySoundFromStream(AudioStream stream)
	{
		if (stream == null) return -1;
		if (!soundPlayer.Playing) soundPlayer.Play();
		if (soundPlayer.GetStreamPlayback() is AudioStreamPlaybackPolyphonic playback)
		{
			return playback.PlayStream(stream);
		}
		return -1;
	}

	public static long PlaySoundFromPath(string path)
	{
		Resource res = UtmxResourceLoader.Load(path);
		if (res != null && res is AudioStream stream)
		{
			return PlaySoundFromStream(stream);
		}
		else
		{
			UtmxLogger.Error($"{TranslationServer.Translate("Cannot play sound: no valid audio file found")}: {path}");
		}
		return -1;
	}

	public static void StopSound(long id)
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


	public static void PlayBgmFromStream(string bgmId, AudioStream stream, bool loop = false)
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
					player.Play(0);
				}
				else
				{
					StopBgm(bgmId);
				}
			};
			bgmPlayers.Add(bgmId, player);
			Instance.AddChild(player);
		}
		player.Stream = stream;
        player.Bus = "Bgm";
        player.Play(0);
		player.SetMeta("loop", loop);
	}

	public static void PlayBgmFromPath(string bgmId, string path, bool loop = false)
	{
		if (string.IsNullOrEmpty(bgmId)) return;
		Resource res = UtmxResourceLoader.Load(path);
		if (res != null && res is AudioStream stream)
		{
			PlayBgmFromStream(bgmId, stream, loop);
		}
	}
	public static void StopBgm(string bgmId)
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
		UtmxLogger.Error($"Bgm player with id '{bgmId}' not found.");
	}
	public static void StopAll()
	{
		soundPlayer.Stop();
		foreach (string playerId in bgmPlayers.Keys)
		{
			StopBgm(playerId);
		}
	}

	public static void SetBgmPaused(string bgmId, bool paused)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			player.StreamPaused = paused;
			return;
		}
		UtmxLogger.Error($"Bgm player with id '{bgmId}' not found.");
	}
	public static bool GetBgmPaused(string bgmId)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			return player.StreamPaused;
		}
		UtmxLogger.Error($"Bgm player with id '{bgmId}' not found.");
        return false;
    }

	public double GetBgmVolume(string bgmId)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			return player.VolumeDb;
		}
		UtmxLogger.Error($"Bgm player with id '{bgmId}' not found.");
        return 0;
    }

	public static void SetBgmVolume(string bgmId, double volumeDb, double duration = 0)
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
				player.VolumeDb = (float)volumeDb;
			}
			return;
		}
		UtmxLogger.Error($"Bgm player with id '{bgmId}' not found.");
	}
	public static float GetBgmPitch(string bgmId)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			return player.PitchScale;
		}
		UtmxLogger.Error($"Bgm player with id '{bgmId}' not found.");
		return 0;
	}
	public static void SetBgmPitch(string bgmId, double pitch, double duration = 0)
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
				player.PitchScale = (float)pitch;
			}
			return;
		}
		UtmxLogger.Error($"Bgm player with id '{bgmId}' not found.");
    }
    public static double GetBgmPosition(string bgmId)
	{
		if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
		{
			return player.GetPlaybackPosition();
		}
		UtmxLogger.Error($"Bgm player with id '{bgmId}' not found.");
		return 0;
	}

    public static void SetBgmPosition(string bgmId, double position)
    {
        if (bgmPlayers.TryGetValue(bgmId, out AudioStreamPlayer player))
        {
            player.Play((float)position);
        }
    }
	public static void AppendStreamToLibrary(string id, AudioStream stream)
	{
		if (stream == null) return;
		if (!_streamLibrary.TryGetValue(id, out _))
		{
			_streamLibrary[id] = stream;
		}
	}

	public static void RemoveStreamToLibrary(string id)
	{
		if (_streamLibrary.TryGetValue(id, out _))
		{
			_streamLibrary.Remove(id);
		}
	}

	public static AudioStream GetStreamFormLibrary(string id)
	{
		return _streamLibrary[id];
	}
}
