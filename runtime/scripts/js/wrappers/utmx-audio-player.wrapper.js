import { __audio_player } from "__UTMX";

export class UtmxAudioPlayer {
	static playSound(soundFilePath) {
		return __audio_player.PlaySoundFromPath(soundFilePath);
	}

	static playBgm(bgmId, bgmFilePath, loop = false) {
		__audio_player.PlayBgmFromPath(bgmId, bgmFilePath, loop);
	}

	static isBgmValid(bgmId) {
		return __audio_player.IsBgmValid(bgmId);
	}

	static stopSound(soundId) {
		return __audio_player.StopSound(soundId);
	}

	static setSoundVolume(soundId, volume) {
		return __audio_player.SetSoundVolume(soundId, volume);
	}

	static setSoundPitch(soundId, pitch) {
		return __audio_player.SetSoundPitch(soundId, pitch);
	}

	static stopBgm(bgmId) {
		return __audio_player.StopBgm(bgmId);
	}

	static stopAll() {
		return __audio_player.StopAll();
	}

	static getBgmVolume(bgmId) {
		return __audio_player.GetBgmVolume(bgmId);
	}

	static setBgmVolume(bgmId, volume, duration = 0) {
		return __audio_player.SetBgmVolume(bgmId, volume, duration);
	}

	static getBgmPitch(bgmId) {
		return __audio_player.GetBgmPitch(bgmId);
	}

	static setBgmPitch(bgmId, pitch, duration = 0) {
		return __audio_player.SetBgmPitch(bgmId, pitch, duration);
	}
	
	static getBgmPaused(bgmId) {
		return __audio_player.GetBgmPaused(bgmId);
	}
	static setBgmPaused(bgmId, paused) {
		return __audio_player.SetBgmPaused(bgmId, paused);
	}

	static getBgmPosition(bgmId) {
		return __audio_player.GetBgmPosition(bgmId);
	}

	static setBgmPosition(bgmId, position) {
		return __audio_player.SetBgmPosition(bgmId, position);
	}
}
