using Godot;
using System;

public partial class BattleAttackAnimationFrame : BattleAttackAnimation
{
    static AudioStream attackSound = UtmxGlobalStreamPlayer.GetStreamFormLibrary("LAZ");

    public override void _Ready()
    {
        SetLoop(false);
        UtmxGlobalStreamPlayer.PlaySoundFromStream(attackSound);
        Play();
        SpeedScale = 2.0F;
        Connect(AnimatedSprite2D.SignalName.AnimationFinished,
            Callable.From(() =>
            {
                EmitSignal(SignalName.Finished, []);
                QueueFree();
            }
        ));
    }
}
