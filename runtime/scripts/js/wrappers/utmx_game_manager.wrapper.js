import { __game_manager } from "__UTMX";


export class UtmxGameManager 
{
    static getFpsReal()
    {
        return __game_manager.GetFpsReal();
    }
    static setMaxFps(fps)
    {
        return __game_manager.SetMaxFps(fps);
    }

    static quitGame()
    {
        __game_manager.QuitGame();
    }
}