using Godot;
using System;

[GlobalClass]
public abstract partial class BaseEncounterMenu : Control
{
    public abstract void UIVisible();
    public abstract void UIHidden();
}
