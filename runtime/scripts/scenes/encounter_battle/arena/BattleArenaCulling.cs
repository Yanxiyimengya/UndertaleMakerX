using Godot;
using System;

[GlobalClass]
public abstract partial class BattleArenaCulling : BaseBattleArena
{
    public override bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            Visible = value;
            if (!Engine.IsEditorHint())
            {
                if (value)
                {
                    PhysicsServer2D.BodySetSpace(_arenaPhysicBody, GetWorld2D().Space);
                }
                else
                {
                    PhysicsServer2D.BodySetSpace(_arenaPhysicBody, EmptyRid);
                }
            }
        }
    }

    protected Rid _arenaPhysicBody;
    protected Rid _shape;
    private static readonly Rid EmptyRid = new Rid();
    protected abstract Rid GenerateCollisionShape();
    protected abstract void UpdateCollisionShape(Rid shape);

    public BattleArenaCulling()
    {
        _arenaPhysicBody = PhysicsServer2D.BodyCreate();
        PhysicsServer2D.BodySetMode(_arenaPhysicBody, Engine.IsEditorHint() ?
            PhysicsServer2D.BodyMode.Static : PhysicsServer2D.BodyMode.Kinematic);
        PhysicsServer2D.BodySetCollisionLayer(_arenaPhysicBody, (int)UtmxBattleManager.BattleCollisionLayers.Player);
        PhysicsServer2D.BodySetCollisionMask(_arenaPhysicBody, (int)UtmxBattleManager.BattleCollisionLayers.Player);
        _shape = GenerateCollisionShape();
        if (_shape.IsValid)
        {
            PhysicsServer2D.BodyAddShape(_arenaPhysicBody, _shape);
            UpdateCollisionShape(_shape);
        }
    }
    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            if (_shape.IsValid) PhysicsServer2D.FreeRid(_shape);
            if (_arenaPhysicBody.IsValid)  PhysicsServer2D.FreeRid(_arenaPhysicBody);
        }
    }

    public override void _EnterTree()
    {
        PhysicsServer2D.BodySetSpace(_arenaPhysicBody, GetWorld2D().Space);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) return;
        if (Enabled)
        {
            PhysicsServer2D.BodySetState(_arenaPhysicBody, PhysicsServer2D.BodyState.Transform, GlobalTransform);
        }
    }
}
