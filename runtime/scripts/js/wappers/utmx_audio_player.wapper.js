import {__audio_player } from "__UTMX";

export class UtmxAudioPlayer
{
	static playSound(soundFilePath)
	{
		return __audio_player.Instance.PlaySoundFromPath(soundFilePath);
	}
	
	static playBgm(bgmId, bgmFilePath, loop = false)
	{
		__audio_player.Instance.PlayBgmFromPath(bgmId, bgmFilePath, loop);
	}

	static stopSound(soundId)
	{
		return __audio_player.Instance.StopSound(soundId);
	}

	static stopBgm(bgmId)
	{
		return __audio_player.Instance.StopBgm(bgmId);
	}

	static stopAll()
	{
		return __audio_player.Instance.StopAll();
	}

	static getBgmVolume(bgmId)
	{
		return __audio_player.Instance.GetBgmVolume(bgmId);
	}

	static setBgmVolume(bgmId, volume, duration = 0)
	{
		return __audio_player.Instance.SetBgmVolume(bgmId, volume, duration);
	}

	static getBgmPitch(bgmId)
	{
		return __audio_player.Instance.GetBgmPitch(bgmId);
	}

	static setBgmPitch(bgmId, pitch, duration = 0)
	{
		return __audio_player.Instance.SetBgmPitch(bgmId, pitch, duration);
	}
}
