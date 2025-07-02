using System;
using System.Reflection;
using Godot;

namespace Metroidarium;

public class TweenValues
{
    public float Delay;
    public Tween.EaseType Ease;
    public Tween.TransitionType Transition;
}

public class TweenComponent(Entity actor) : Component
{
    private Entity actor = actor;
    private SceneTree tree = actor.GetTree();

    public void To(string property, Variant finalVal, double duration, object values=null, Callable onEnd=new())
    {
        Tween tw = tree.CreateTween();
        PropertyTweener propTweener = tw.TweenProperty(actor, property, finalVal, duration);
        tw.TweenCallback(onEnd);

        if (values != null)
        {
            TweenValues tv = fromAnonymous(values);
            // Do I need to check for null?
            propTweener.SetEase(tv.Ease);
            propTweener.SetTrans(tv.Transition);
            propTweener.SetDelay(tv.Delay);
        }
        
    }
    
    private TweenValues fromAnonymous(object anon)
    {
        TweenValues values = new TweenValues();
        Type anonType = anon.GetType();
        Type valuesType = typeof(TweenValues);

        foreach (PropertyInfo prop in anonType.GetProperties())
        {
            // Find matching property in TweenValues by name 
            PropertyInfo tweenProp = valuesType.GetProperty(prop.Name);
            if (tweenProp != null && tweenProp.CanWrite)
            {
                var value = prop.GetValue(anon);
                tweenProp.SetValue(values, value);
            }
        }

        return values;
    }


}