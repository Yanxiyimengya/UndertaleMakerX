using Acornima;
using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Native.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Godot.Tween;

public partial class JavaScriptTweenManager : Node
{
    public static JavaScriptTweenManager Instance;
    public static HashSet<JavaScriptTween> TweenList = new();
    public override void _EnterTree()
    {
        if (Instance != null)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }
    public static JavaScriptTween CreateJavaScriptTween()
    {
        if (Instance != null)
        {
            Tween tween = Instance.CreateTween();
            JavaScriptTween propertyTween = new JavaScriptTween(tween);
            TweenList.Add(propertyTween);
            return propertyTween;
        }
        return null;
    }

    public static JavaScriptTween[] getTweenList()
    {
        return TweenList.ToArray();
    }
}

public partial class JavaScriptTween : RefCounted
{
    Tween tween;
    public static HashSet<JavaScriptTweener> TweenerList = new();
    public JavaScriptTween(Tween @tween)
    {
        this.tween = @tween;
        @tween.Finished += kill;
    }
    public JavaScriptTweenerProperty addTweenProperty(ObjectInstance ins, string propName, JsValue finalValue, double duration)
    {
        if (ins.Get("__instance").ToObject() is Node targetNode)
            tween.BindNode(targetNode);
        object value = finalValue.ToObject();
        JavaScriptTweenerProperty javaScriptTweenerProperty = new JavaScriptTweenerProperty(ins);
        PropertyTweener propertyTweener = value switch
        {
            double v => tween.TweenProperty(javaScriptTweenerProperty, propName, v, duration),
            Vector2 v => tween.TweenProperty(javaScriptTweenerProperty, propName, v, duration),
            Vector3 v => tween.TweenProperty(javaScriptTweenerProperty, propName, v, duration),
            Vector4 v => tween.TweenProperty(javaScriptTweenerProperty, propName, v, duration),
            Color v => tween.TweenProperty(javaScriptTweenerProperty, propName, v, duration),
            _ => null,
        };
        if (propertyTweener == null) return null;
        javaScriptTweenerProperty.propertyTweener = propertyTweener;
        TweenerList.Add(javaScriptTweenerProperty);
        propertyTweener.Finished += () => TweenerList.Remove(javaScriptTweenerProperty);
        return javaScriptTweenerProperty;
    }
    public void kill()
    {
        tween.Kill();
        TweenerList.Clear();
        JavaScriptTweenManager.TweenList.Remove(this);
        Dispose();
    }
    public void pause() => tween.Pause();
    public void play() => tween.Play();
    public JavaScriptTween setParallel(bool parallel = true)
    {
        tween.SetParallel(parallel);
        return this;
    }
    public JavaScriptTween chain()
    {
        tween.Chain();
        return this;
    }
    public bool isRunning() => tween.IsRunning();
    public JavaScriptTween callback(Action func)
    {
        tween.Finished += func;
        return this;
    }
}

public partial class JavaScriptTweener : RefCounted
{

}

public partial class JavaScriptTweenerProperty : JavaScriptTweener
{
    public ObjectInstance instance;
    public PropertyTweener propertyTweener;
    public JavaScriptTweenerProperty(ObjectInstance ins)
    {
        instance = ins;
    }
    public JavaScriptTweenerProperty from(object value)
    {
        propertyTweener.From(value switch
        {
            double v => v,
            Vector2 v => v,
            Vector3 v => v,
            Vector4 v => v,
            Color v => v,
            _ => new Variant(),
        });
        return this;
    }
    public JavaScriptTweenerProperty trans(ulong trans)
    {
        propertyTweener.SetTrans((Tween.TransitionType)trans);
        return this;
    }
    public JavaScriptTweenerProperty ease(ulong ease)
    {
        propertyTweener.SetEase((Tween.EaseType)ease);
        return this;
    }
    public JavaScriptTweenerProperty delay(double delayTime)
    {
        propertyTweener.SetDelay((float)delayTime);
        return this;
    }
    public JavaScriptTweenerProperty relative()
    {
        propertyTweener.AsRelative();
        return this;
    }
    public JavaScriptTweenerProperty callback(Action func)
    {
        propertyTweener.Finished += func;
        return this;
    }
    public JavaScriptTweenerProperty asRelative()
    {
        propertyTweener.AsRelative();
        return this;
    }
    public override bool _Set(StringName propName, Variant value)
    {
        if (instance == null) return false;

        {
            if (instance != null)
            {
                string prop = propName.ToString();
                if (instance.HasProperty(prop))
                {
                    instance.Set(prop, JsValue.FromObject(JavaScriptBridge.MainEngine, value.Obj));
                    return true;
                }
            }
        }
        return false;
    }
    public override Variant _Get(StringName propName)
    {
        string prop = propName.ToString();
        if (instance.HasProperty(prop))
        {
            JsValue jsValue = instance.Get(prop);
            return JavaScriptBridge.ObjectConvertToVariant(jsValue.ToObject());
        }
        return new Variant();
    }
}

