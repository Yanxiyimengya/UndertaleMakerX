using Godot;
using System;
using System.Linq;

public partial class BootSplash : Control
{
	[Export(PropertyHint.File, "*.tscn")]
	public string FirstScene = "";
	public string TargetAnim = "boot";

	[Export]
	public Label bootAnimationLabelBack;
	[Export]
	public Label bootAnimationLabelFore;
	
	[Export]
	public ColorRect backgroundColorRect;
	
	[Export]
	public AnimationPlayer bootAnimationPlayer;
	
	public override void _Ready()
	{

		FirstScene = UTMXResourceLoader.ResolvePath(
			(string)UTMXRuntimeProjectConfig.Instance.TryGetDefault("application/main_scene", FirstScene)
			);

		var a = UTMXRuntimeProjectConfig.Instance.TryGetDefault("boot_splash/enabled", (Variant)true).AsBool();

        if (!UTMXRuntimeProjectConfig.Instance.TryGetDefault("boot_splash/enabled", (Variant)true).AsBool())
		{
			CallDeferred("Finished");
			return;
		}

		bootAnimationPlayer.Play(TargetAnim);
		bootAnimationPlayer.SpeedScale = (float)UTMXRuntimeProjectConfig.Instance.TryGetDefault("boot_splash/speed_scale",
			bootAnimationPlayer.SpeedScale);
		string displayText = (string)UTMXRuntimeProjectConfig.Instance.TryGetDefault("boot_splash/display_text",
			bootAnimationLabelBack.Text);
		bootAnimationLabelBack.Text = displayText;
		bootAnimationLabelFore.Text = displayText;
		backgroundColorRect.Color = Color.FromString(
			(string)UTMXRuntimeProjectConfig.Instance.TryGetDefault("boot_splash/background_color", ""),
			 backgroundColorRect.Color);
		// 初始化画面元素

		bootAnimationPlayer.Connect(AnimationPlayer.SignalName.AnimationFinished,
			new Callable(this, nameof(OnIntroAnimationPlayerAnimationFinished)));
	}

	private void OnIntroAnimationPlayerAnimationFinished(StringName animName)
	{
		if (TargetAnim != animName) return;
		if (! string.IsNullOrEmpty(animName))
		{
			Finished();
		}
	}

	private void Finished()
	{
		GetTree().ChangeSceneToFile(FirstScene);
	}
}
