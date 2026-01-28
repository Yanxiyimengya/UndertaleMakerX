using Godot;
using System;

public partial class BattleAttackAnimationKnife0 : BattleAttackAnimation
{
    [Export]
    public AnimatedSprite2D animSprite2D;
    [Export]
    public AudioStream attackSound;

    public override void _Ready()
    {
        UtmxGlobalStreamPlayer.Instance.PlaySound(attackSound);
        animSprite2D.Play();
        animSprite2D.Connect(AnimatedSprite2D.SignalName.AnimationFinished,
            Callable.From(() =>
            {
                EmitSignal(SignalName.Finished, []);
                QueueFree();
            }
        ));
    }
}
