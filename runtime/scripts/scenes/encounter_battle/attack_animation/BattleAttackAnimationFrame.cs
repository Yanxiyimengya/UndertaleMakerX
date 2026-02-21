using Godot;
using System;

public partial class BattleAttackAnimationFrame : BattleAttackAnimation
{
	static AudioStream attackSound = null;

	public override void _Ready()
    {
        Loop = false;
        SpeedScale = (float)UtmxPlayerDataManager.Weapon.AttackAnimationSpeed * 2.0F;
		Resource res = UtmxResourceLoader.Load(UtmxPlayerDataManager.Weapon.AttackSound);
        if (res is AudioStream)
            attackSound = res as AudioStream;
        UtmxGlobalStreamPlayer.PlaySoundFromStream(attackSound);
		Play();
        Connect(AnimatedSprite2D.SignalName.AnimationFinished,
			Callable.From(() =>
            {
                EmitSignal(SignalName.Finished, []);
				QueueFree();
			}
		));
	}
}
