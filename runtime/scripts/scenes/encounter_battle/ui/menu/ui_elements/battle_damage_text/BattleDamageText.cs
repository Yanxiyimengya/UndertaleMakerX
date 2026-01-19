using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class BattleDamageText : Node2D
{
	[Signal]
	public delegate void EndedEventHandler();

	[Export]
	TextTyper DamageTextTyper;
	
	public void SetText(string text)
	{
		DamageTextTyper.TyperColor = Color.Color8(0xC0, 0xC0, 0xC0);
		DamageTextTyper.Start(text);
		Start();
	}
	public void SetNumber(int number)
	{
		DamageTextTyper.TyperColor = Colors.Red;
		DamageTextTyper.Start(number.ToString());
		Start();
	}

	private async void Start()
	{
		await ToSignal(GetTree().CreateTimer(1.0), Timer.SignalName.Timeout);
		EmitSignal(SignalName.Ended, []);
		QueueFree();
	}

}
