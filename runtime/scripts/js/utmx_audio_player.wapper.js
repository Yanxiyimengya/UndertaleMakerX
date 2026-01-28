import {__audio_player, __logger} from "__UTMX";

export class UtmxAudioPlayer
{
    static playSound(soundFilePath)
    {
        if (typeof soundFilePath === 'string')
        {
            __logger.Log("Playing sound from path: " + soundFilePath);
            __logger.Log(__audio_player.Instance);
            return __audio_player.Instance.PlaySoundFromPath(soundFilePath);
        }
    }
}