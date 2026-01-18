using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class BaseEnemy : Node2D
{
	public string DisplayName = "ENEMY";

	public float Atk = 0.0F;
	public float Def = 0.0F;
	public float Hp = 100.0F;
	public float MaxHp = 100.0F;
    public bool AllowSpare = true;
    public bool CanSpare = false;

	public Array<string> Actions = [];
	
}
