using Godot;
using System;

[GlobalClass]
public abstract partial class BattleArenaCulling : BaseBattleArena
{
    protected Rid _arenaPhysicBody;
    protected Rid _shape;
    protected abstract Rid GenerateCollisionShape();
    protected abstract void UpdateCollisionShape(Rid shape);

    public BattleArenaCulling()
    {
        _arenaPhysicBody = PhysicsServer2D.BodyCreate();
        PhysicsServer2D.BodySetMode(_arenaPhysicBody, Engine.IsEditorHint() ? 
            PhysicsServer2D.BodyMode.Static : PhysicsServer2D.BodyMode.Kinematic);
        _shape = GenerateCollisionShape();
        if (_shape.IsValid)
        {
            PhysicsServer2D.BodyAddShape(_arenaPhysicBody, _shape);
            UpdateCollisionShape(_shape);
        }
    }
    ~BattleArenaCulling()
    {
        if (_shape.IsValid) PhysicsServer2D.FreeRid(_shape);
        if (_arenaPhysicBody.IsValid) PhysicsServer2D.FreeRid(_arenaPhysicBody);
    }

    public override void _EnterTree()
    {
        PhysicsServer2D.BodySetSpace(_arenaPhysicBody, GetWorld2D().Space);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) return;
        base._PhysicsProcess(delta);
        PhysicsServer2D.BodySetState(_arenaPhysicBody, PhysicsServer2D.BodyState.Transform, GlobalTransform);
    }
}
