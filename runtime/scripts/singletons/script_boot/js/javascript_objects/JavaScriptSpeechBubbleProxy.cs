using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using System.Collections.Generic;

[GlobalClass]
public partial class JavaScriptSpeechBubbleProxy : RefCounted
{
    public static SpeechBubble New(ObjectInstance objInstance)
    {
        SpeechBubble bubble = UtmxSceneManager.CreateSpeechBubble();
        if (bubble == null) return null;

        TextTyper typer = bubble.SpeechBubbleTextTyper;
        if (typer == null)
        {
            UtmxSceneManager.DeleteSpeechBubble(bubble);
            return null;
        }

        typer.ProcessCmdCallback = (string cmd, Dictionary<string, string> args) =>
        {
            if (objInstance == null || !objInstance.HasProperty("processCmd"))
                return false;

            JsValue result = JavaScriptBridge.InvokeFunction(objInstance, "processCmd", [cmd, args]);
            if (result == null)
                return false;

            if (result.IsNull() || result.IsUndefined())
                return false;

            return result.AsBoolean();
        };

        return bubble;
    }
}
