using Godot;

public partial class GameoverScene : Node
{
    [Export]
    Texture2D SoulBreakTexture;

    [Export]
    Node2D SoulPositionNode;
    [Export]
    Sprite2D SoulSprite2D;
    [Export]
    Sprite2D GameoverBg;
    [Export]
    GpuParticles2D SoulParticles2D;
    [Export]
    TextTyper GameoverTextTyper;

    private bool _inputAcceptable = false;

    public override void _Ready()
    {
        _inputAcceptable = false;
        Play();
    }

    public override void _Process(double delta)
    {
        if (_inputAcceptable)
        {
            if (Input.IsActionJustPressed("confirm"))
            {
                if (GameoverTextTyper.IsFinished())
                {
                    End();
                }
            }
        }
    }


    public override void _ExitTree()
    {
        if (UtmxGlobalStreamPlayer.IsBgmValid("_GAME_OVER"))
            UtmxGlobalStreamPlayer.StopBgm("_GAME_OVER");
    }

    private async void Play()
    {
        UtmxGlobalStreamPlayer.StopAll();
        UtmxGlobalStreamPlayer.PlayBgmFromStream("_GAME_OVER", UtmxGlobalStreamPlayer.GetStreamFormLibrary("GAME_OVER"), true);
        UtmxGlobalStreamPlayer.SetBgmPaused("_GAME_OVER", true);

        UtmxBattleManager.GetEncounterInstance()?._OnGameover();
        _inputAcceptable = false;
        SoulSprite2D.Visible = true;
        GameoverTextTyper.Visible = false;
        SoulPositionNode.Modulate = UtmxBattleManager.PlayerSoulColor;
        SoulPositionNode.GlobalTransform = UtmxBattleManager.PlayerSoulTransform;
        GameoverBg.Modulate = Color.Color8(255, 255, 255, 0);
        await ToSignal(GetTree().CreateTimer(0.67), Timer.SignalName.Timeout);

        SoulSprite2D.Texture = SoulBreakTexture;
        UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("HEART_BEAT_BREAK"));
        await ToSignal(GetTree().CreateTimer(1.5), Timer.SignalName.Timeout);

        SoulSprite2D.Visible = false;
        UtmxGlobalStreamPlayer.PlaySoundFromStream(UtmxGlobalStreamPlayer.GetStreamFormLibrary("HEART_PLOSION"));
        for (int i = 0; i < 4; i++)
        {
            SoulParticles2D.EmitParticle(SoulParticles2D.GlobalTransform, Vector2.Zero, Colors.White,
                Colors.White, (uint)GpuParticles2D.EmitFlags.Position);
        }
        await ToSignal(GetTree().CreateTimer(1.75), Timer.SignalName.Timeout);

        Tween _tween = CreateTween();
        _tween.TweenProperty(GameoverBg, "modulate:a", 1.0, 1.0).From(0.0);
        if (UtmxGlobalStreamPlayer.IsBgmValid("_GAME_OVER"))
            UtmxGlobalStreamPlayer.SetBgmPaused("_GAME_OVER", false);
        await ToSignal(_tween, Tween.SignalName.Finished);

        GameoverTextTyper.Visible = true;
        GameoverTextTyper.Start(UtmxBattleManager.GetEncounterInstance()?.DeathText);
        _inputAcceptable = true;
    }

    private async void End()
    {
        UtmxPlayerDataManager.ResetPlayerState();
        UtmxGlobalStreamPlayer.SetBgmVolume("_GAME_OVER", -72, 1.0F);
        _inputAcceptable = false;
        GameoverTextTyper.Visible = false;
        Tween _tween = CreateTween();
        _tween.TweenProperty(GameoverBg, "modulate:a", 0.0, 1.0).From(1.0);
        await ToSignal(_tween, Tween.SignalName.Finished);
        UtmxBattleManager.EndEncounterBattle();
    }
}
