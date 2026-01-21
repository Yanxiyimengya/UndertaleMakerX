using Godot;
using System;

public partial class BattleAttackAnimationKnife0 : BattleAttackAnimation
{
	[Export]
	public AnimatedSprite2D animSprite2D;

	public override void _Ready()
	{
		animSprite2D.Play();
		animSprite2D.Connect(AnimatedSprite2D.SignalName.AnimationFinished, 
			Callable.From(() => {
				EmitSignal(SignalName.Finished, []);
				QueueFree();
			}
		));
	}
}
