using Godot;
using Jint.Native;
using Jint.Native.Object;

[GlobalClass]
public partial class JavaScriptLifecycleProxy : Node
{
	public ObjectInstance JsInstance {
		get => _jsInstance;
		set
		{
			if (ReferenceEquals(_jsInstance, value))
				return;

			_jsInstance = value;
			if (_jsInstance == null)
			{
				_startedInstance = null;
				_startInvokeScheduled = false;
				SetProcess(false);
				return;
			}

			SetProcess(_jsInstance.HasProperty(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK));
			if (IsInsideTree())
				ScheduleStartInvoke();
		}
	}
	private ObjectInstance _jsInstance = null;
	private ObjectInstance _startedInstance = null;
	private bool _startInvokeScheduled = false;

	public override void _EnterTree()
	{
		CallDeferred(nameof(OnEnterTree));
	}

	public override void _Process(double delta)
	{
		Invoke(EngineProperties.JAVASCRIPT_UPDATE_CALLBACK, delta);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationPredelete)
			Invoke(EngineProperties.JAVASCRIPT_DESTROY_CALLBACK);
	}

	public JsValue Invoke(string method, params object[] args)
	{
		if (JsInstance == null || string.IsNullOrEmpty(method))
			return null;
		if (JsInstance.HasProperty(method))
			return JavaScriptBridge.InvokeFunction(JsInstance, method, args);
		return null;
	}

	private void OnEnterTree()
	{
		Node parent = GetParent();
		if (parent is IJavaScriptLifecyucle lc && !ReferenceEquals(JsInstance, lc.JsInstance))
		{
			JsInstance = lc.JsInstance;
		}
		ScheduleStartInvoke();
	}

	private void ScheduleStartInvoke()
	{
		if (JsInstance == null || ReferenceEquals(_startedInstance, JsInstance) || _startInvokeScheduled)
			return;

		_startInvokeScheduled = true;
		CallDeferred(nameof(InvokeStartIfNeeded));
	}

	private void InvokeStartIfNeeded()
	{
		_startInvokeScheduled = false;
		if (!IsInsideTree())
			return;

		if (JsInstance == null || ReferenceEquals(_startedInstance, JsInstance))
			return;

		_startedInstance = JsInstance;
		Invoke(EngineProperties.JAVASCRIPT_START_CALLBACK);
	}
}
