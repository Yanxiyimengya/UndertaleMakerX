using Godot;
using System;

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
		GlobalStreamPlayer.Instance.StopBGM("GAME_OVER");
	}

	private async void Play()
	{
		_inputAcceptable = false;
		GlobalStreamPlayer.Instance.StopAll();
		SoulSprite2D.Visible = true;
		GameoverTextTyper.Visible = false;
		SoulPositionNode.Modulate = GlobalBattleManager.Instance.PlayerSoulColor;
		SoulPositionNode.GlobalPosition = GlobalBattleManager.Instance.PlayerSoulPosition;
		GameoverBg.Modulate = Color.Color8(255, 255, 255, 0);
		await ToSignal(GetTree().CreateTimer(0.67), Timer.SignalName.Timeout);

		SoulSprite2D.Texture = SoulBreakTexture;
		GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStreamFormLibrary("HEART_BEAT_BREAK"));
		await ToSignal(GetTree().CreateTimer(1.5), Timer.SignalName.Timeout);

		SoulSprite2D.Visible = false;
		GlobalStreamPlayer.Instance.PlaySound(GlobalStreamPlayer.Instance.GetStreamFormLibrary("HEART_PLOSION"));
		for (int i = 0; i < 4; i ++)
		{
			SoulParticles2D.EmitParticle(SoulParticles2D.GlobalTransform, Vector2.Zero, Colors.White,
				Colors.White, (uint)GpuParticles2D.EmitFlags.Position);
		}
		await ToSignal(GetTree().CreateTimer(1.75), Timer.SignalName.Timeout);

		Tween _tween = CreateTween();
		_tween.TweenProperty(GameoverBg, "modulate:a", 1.0, 1.0).From(0.0);
		GlobalStreamPlayer.Instance.PlayBGM("GAME_OVER", GlobalStreamPlayer.Instance.GetStreamFormLibrary("GAME_OVER"));
		await ToSignal(_tween, Tween.SignalName.Finished);

		GameoverTextTyper.Visible = true;
		GameoverTextTyper.Start(GlobalBattleManager.Instance.DeathText);
		_inputAcceptable = true;
	}

	private async void End()
	{
		GlobalStreamPlayer.Instance.SetBgmVolume("GAME_OVER", -72, true, 2.0);
		_inputAcceptable = false;
		GameoverTextTyper.Visible = false;
		Tween _tween = CreateTween();
		_tween.TweenProperty(GameoverBg, "modulate:a", 0.0, 1.0).From(1.0);
		await ToSignal(_tween, Tween.SignalName.Finished);

	}
}
