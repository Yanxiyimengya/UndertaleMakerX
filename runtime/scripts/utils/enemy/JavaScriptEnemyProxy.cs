using Godot;

public partial class JavaScriptEnemyProxy : BaseEnemy
{
    public JavaScriptObjectInstance JsInstance;
    public override void _OnSpare()
    {
        JsInstance.Invoke("onSpare", []);
    }

    public override void _HandleAction(string action)
    {
        JsInstance.Invoke("handleAction", [action]);
    }
    public override void _HandleAttack(AttackStatus status)
    {
        JsInstance.Invoke("handleAttack", [status]);
    }
}
