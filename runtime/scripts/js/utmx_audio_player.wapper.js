import {__audio_player, __logger} from "__UTMX";

export class UtmxAudioPlayer
{
    static playSound(soundFilePath)
    {
        if (typeof soundFilePath === 'string')
        {
            return __audio_player.Instance.PlaySoundFromPath(soundFilePath);
        }
        return -1;
    }
    
    static playBgm(bgmId, bgmFilePath, loop = false)
    {
        if (typeof bgmFilePath === 'string')
        {
            __audio_player.Instance.PlayBgmFromPath(bgmId, bgmFilePath, loop);
        }
    }

    static stopSound(soundId)
    {
        if (typeof soundId === 'number')
        {
            return __audio_player.Instance.StopSound(soundId);
        }
        return -1;
    }

    static stopBgm(bgmId)
    {
        if (typeof bgmId === 'string')
        {
            return __audio_player.Instance.StopBgm(bgmId);
        }
        return -1;
    }

    static stopAll()
    {
        return __audio_player.Instance.StopAll();
    }

    static getBgmVolume(bgmId)
    {
        if (typeof bgmId === 'string')
        {
            return __audio_player.Instance.GetBgmVolume(bgmId);
        }
        return -1;
    }

    static setBgmVolume(bgmId, volume, duration = 0)
    {
        if (typeof bgmId === 'string')
        {
            return __audio_player.Instance.SetBgmVolume(bgmId, volume, duration);
        }
        return -1;
    }

    static getBgmPitch(bgmId)
    {
        if (typeof bgmId === 'string')
        {
            return __audio_player.Instance.GetBgmPitch(bgmId);
        }
        return -1;
    }

    static setBgmPitch(bgmId, pitch, duration = 0)
    {
        if (typeof bgmId === 'string')
        {
            return __audio_player.Instance.SetBgmPitch(bgmId, pitch, duration);
        }
        return -1;
    }
}