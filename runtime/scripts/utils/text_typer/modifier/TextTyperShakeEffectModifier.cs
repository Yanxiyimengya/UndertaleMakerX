using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[GlobalClass]
public partial class TextTyperShakeEffectModifier : TextTyperEffectModifier
{
    public override void _Update(Rid canvasItem)
    {
        RenderingServer.CanvasItemSetTransform(canvasItem, Transform2D.Identity.Translated(new Vector2(
            5 * GD.Randf(),
            5 * GD.Randf()
            )));
    }
}
