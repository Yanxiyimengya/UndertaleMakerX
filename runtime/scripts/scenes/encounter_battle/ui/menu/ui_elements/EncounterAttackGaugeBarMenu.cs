using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class EncounterAttackGaugeBarMenu : BaseEncounterMenu
{
	[Signal]
	delegate void HittedEventHandler(bool missed, float value);

	[Export]
	Sprite2D TargetSprite2D;
	[Export]
	AnimatedSprite2D TargetChoiceBar;

	private int _dir = 0;
	private bool _started = false;
	private Tween _tween;

	public void Start()
	{
		Array<int> choiceList = new Array<int> { -1, 1 };
		_dir = choiceList.PickRandom();
		_started = true;
		TargetSprite2D.Visible = true;
		TargetSprite2D.Modulate = Colors.White;
		TargetSprite2D.Scale = Vector2.One;
		TargetChoiceBar.Visible = true;
		TargetChoiceBar.Modulate = Colors.White;

		_tween?.Kill();
		_tween = CreateTween();
		float startX = TargetSprite2D.Position.X + (_dir * 320);
		float endX = TargetSprite2D.Position.X - (_dir * 310);
		_tween.TweenProperty(TargetChoiceBar, "position:x", endX, 1.7F).From(startX);
		_tween.TweenCallback(Callable.From(() =>
		{
			EmitSignal(SignalName.Hitted, [true, -1]);
		}));
	}

	public void Hit()
	{
		if (!_started) return;
		_tween?.Kill();
		TargetChoiceBar.Play();
		TargetSprite2D.Scale = new Vector2(1.0F, TargetSprite2D.Scale.Y);
		TargetSprite2D.Modulate = Colors.White;
		float hitOffset = Math.Abs(TargetSprite2D.Position.X - TargetChoiceBar.Position.X);
		EmitSignal(SignalName.Hitted, [false, hitOffset]);
	}

	public async Task End()
	{
		_started = false;
		TargetChoiceBar.Visible = false;
		_tween?.Kill();
		_tween = CreateTween();
		_tween.SetParallel();
		_tween.TweenProperty(TargetSprite2D, "scale:x", 0.0F, 0.4F).From(1.0F);
		_tween.TweenProperty(TargetSprite2D, "modulate:a", 0.0F, 0.4F).From(1.0F);
		_tween.Chain();
		await ToSignal(_tween, Tween.SignalName.Finished);
	}

	public override void UIVisible() {
		Start();
	}
	public override void UIHidden() { }

}
