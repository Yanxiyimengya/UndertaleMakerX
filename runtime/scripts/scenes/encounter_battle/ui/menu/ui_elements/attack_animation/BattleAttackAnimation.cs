using Godot;
using System;

[GlobalClass]
public abstract partial class BattleAttackAnimation : Node2D
{
    [Signal]
    public delegate void FinishedEventHandler();


    public override void _Ready()
    {
        EmitSignal(SignalName.Finished, []);
    }
}
