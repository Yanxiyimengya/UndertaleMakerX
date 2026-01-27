using Godot;
using System;

public partial class BattleInterfaces : Node
{
	[Export]
	public BattleUiInterface UI;
	[Export]
	public BattleCameraInterface Camera;
	[Export]
	public BattleArenaInterface Arena;
	[Export]
	public BattlePlayerInterface Player;
}
