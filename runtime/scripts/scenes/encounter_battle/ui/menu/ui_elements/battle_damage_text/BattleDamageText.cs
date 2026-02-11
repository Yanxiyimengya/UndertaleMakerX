using Godot;

using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class BattleDamageText : Node2D
{
    [Signal]
    public delegate void EndedEventHandler();

    [Export]
    public TextTyper DamageTextTyper;
    public double Ystart { get => _ystart; set => _ystart = value; }


    private double _ystart = 0.0F;
    private double vspeed = -140;
    private double gravity = 600;
    private double waitting = 0.75;

    public override void _Process(double delta)
    {
        if (Position.Y <= Ystart)
        {
            Position = new Vector2(Position.X, Position.Y + (float)(vspeed * delta));
            vspeed += gravity * delta;
        }
        else
        {
            if (waitting > 0)
            {
                waitting -= delta;
            }
            else
            {
                End();
                QueueFree();
            }
            gravity = 0;
            vspeed = 0;
        }
    }

    public void SetText(string text)
    {
        DamageTextTyper.TyperColor = Color.Color8(0xC0, 0xC0, 0xC0);
        DamageTextTyper.Instant = true;
        DamageTextTyper.Start(text);
    }
    public void SetNumber(int number)
    {
        DamageTextTyper.TyperColor = Colors.Red;
        DamageTextTyper.Instant = true;
        DamageTextTyper.Start(number.ToString());
    }

    public void End()
    {
        EmitSignal(SignalName.Ended, []);
    }
    public void Start(Vector2 position)
    {
        Position = position;
        _ystart = position.Y;
    }
}
