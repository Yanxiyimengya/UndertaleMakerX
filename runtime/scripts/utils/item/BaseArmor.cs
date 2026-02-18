using Godot;


[GlobalClass]
public partial class BaseArmor : BaseItem
{
    [Export]
    public float Defence = 0.0F;

    public virtual double onDefend(double value)
    {
        return value;
    }
}
