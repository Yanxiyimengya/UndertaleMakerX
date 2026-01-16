using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class TextTyperEffectModifier : Resource
{
    public bool Enabled = true;
    public List<Rid> Rids = new List<Rid>(); // 需要被修改的打字机Rid
    public abstract void _Update(Rid canvasItem);
}
