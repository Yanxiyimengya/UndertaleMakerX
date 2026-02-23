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

    private static void EnsureSoundPlayer()
    {
        if (soundPlayer != null && IsInstanceValid(soundPlayer))
            return;

        soundPlayer = new AudioStreamPlayer
        {
            Bus = "Sound",
            Stream = new AudioStreamPolyphonic()
        };
    }

    private static void CleanupPlayer(AudioStreamPlayer player, bool keepForPool)
    {
        if (player == null || !IsInstanceValid(player))
            return;

        if (player.HasMeta("volume_tween"))
        {
            Tween tween = (Tween)player.GetMeta("volume_tween");
            if (tween != null && tween.IsValid()) tween.Kill();
            player.RemoveMeta("volume_tween");
        }
        if (player.HasMeta("pitch_tween"))
        {
            Tween tween = (Tween)player.GetMeta("pitch_tween");
            if (tween != null && tween.IsValid()) tween.Kill();
            player.RemoveMeta("pitch_tween");
        }

        player.StreamPaused = false;
        player.Stop();
        player.Stream = null;
        player.ProcessMode = keepForPool ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;
    }

    private static void ReleasePlayer(AudioStreamPlayer player)
    {
        CleanupPlayer(player, keepForPool: false);
        if (player == null || !IsInstanceValid(player))
            return;

        if (player.GetParent() != null)
            player.GetParent().RemoveChild(player);
        player.QueueFree();
    }

    private void ClearExportedStreams()
    {
        SelectSoundStream = null;
        SqueakSoundStream = null;
        EscapedSoundStream = null;
        TextTyperSoundStream = null;
        EnemyDialogueSoundStream = null;
        HurtSoundStream = null;
        HealSoundStream = null;
        LazSoundStream = null;
        HeartBeatBreakSoundStream = null;
        HeartPlosionSoundStream = null;
        GameOverMusicSoundStream = null;
    }

    private void CleanupOnExit()
    {
        StopAll();

        var activePlayers = new List<AudioStreamPlayer>(bgmPlayers.Values);
        bgmPlayers.Clear();
        foreach (AudioStreamPlayer player in activePlayers)
        {
            ReleasePlayer(player);
        }

        while (bgmPlayersPool.Count > 0)
        {
            ReleasePlayer(bgmPlayersPool.Dequeue());
        }

        if (soundPlayer != null && IsInstanceValid(soundPlayer))
        {
            CleanupPlayer(soundPlayer, keepForPool: false);
            if (soundPlayer.GetParent() != null)
                soundPlayer.GetParent().RemoveChild(soundPlayer);
            soundPlayer.QueueFree();
        }
        soundPlayer = null;

        _streamLibrary.Clear();
        ClearExportedStreams();
        Instance = null;
    }

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
        EnsureSoundPlayer();
        if (soundPlayer.GetParent() != this)
        {
            if (soundPlayer.GetParent() != null)
                soundPlayer.GetParent().RemoveChild(soundPlayer);
            AddChild(soundPlayer);
        }

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
        CleanupOnExit();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            CleanupOnExit();
        }
    }

    public static long PlaySoundFromStream(AudioStream stream)
    {
        if (stream == null) return -1;
        EnsureSoundPlayer();
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
        if (soundPlayer != null && IsInstanceValid(soundPlayer) && soundPlayer.Playing)
        {
            if (soundPlayer.GetStreamPlayback() is AudioStreamPlaybackPolyphonic playback)
            {
                playback.StopStream(id);
                return;
            }
        }
        throw new ArgumentException($"Sound with id '{id}' not found.");
    }
    public static void SetSoundVolume(long id, float volume)
    {
        EnsureSoundPlayer();
        if (!soundPlayer.Playing) soundPlayer.Play();
        if (soundPlayer.GetStreamPlayback() is AudioStreamPlaybackPolyphonic playback)
        {
            playback.SetStreamVolume(id, volume);
        }
    }
    public static void SetSoundPitch(long id, float pitch)
    {
        EnsureSoundPlayer();
        if (!soundPlayer.Playing) soundPlayer.Play();
        if (soundPlayer.GetStreamPlayback() is AudioStreamPlaybackPolyphonic playback)
        {
            playback.SetStreamPitchScale(id, pitch);
        }
    }

    public static void PlayBgmFromStream(string bgmId, AudioStream stream, bool loop = false)
    {
        if (string.IsNullOrEmpty(bgmId)) return;
        AudioStreamPlayer player;
        if (bgmPlayers.TryGetValue(bgmId, out player))
        {
            CleanupPlayer(player, keepForPool: true);
            player.ProcessMode = Node.ProcessModeEnum.Inherit;
            player.VolumeDb = 0.0F;
            player.PitchScale = 1.0F;
        }
        else if (bgmPlayersPool.Count > 0)
        {
            player = bgmPlayersPool.Dequeue();
            player.ProcessMode = Node.ProcessModeEnum.Inherit;
            player.VolumeDb = 0.0F;
            player.PitchScale = 1.0F;
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
            CleanupPlayer(player, keepForPool: true);
            bgmPlayersPool.Enqueue(player);
            return;
        }
    }
    public static void StopAll()
    {
        if (soundPlayer != null && IsInstanceValid(soundPlayer))
        {
            soundPlayer.Stop();
        }

        var ids = new List<string>(bgmPlayers.Keys);
        foreach (string playerId in ids)
        {
            StopBgm(playerId);
        }
    }

    public static bool IsBgmValid(string bgmId)
    {
        return bgmPlayers.ContainsKey(bgmId);
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
