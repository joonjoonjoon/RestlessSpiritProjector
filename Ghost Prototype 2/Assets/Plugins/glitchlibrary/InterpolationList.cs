using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class InterpolationList
{
    public List<InterpolationKeyFrame> list = new List<InterpolationKeyFrame>();

    [SerializeField]
    [HideInInspector]
    private AnimationCurve curve = new AnimationCurve();

    public void UpdateInternalAnimationCurve(AnimationCurve anim)
    {
        curve = anim;
    }

    public float Evaluate(float input)
    {
        if (list.Count == 0)
            return 0f;

        return curve.Evaluate(input);

//
//        var last = list.Last();
//        if (input >= last.time)
//        {
//            return last.value;
//        }
//
//        var first = list.First();
//
//        if (input <= first.time)
//        {
//            return first.value;
//        }
//
//        var lowerIndex = list.FindLastIndex(item => item.time < input);
//
//        var lower = list[lowerIndex];
//        var higher = list[lowerIndex + 1];
//
//        var factor = (input - lower.time) / (higher.time - lower.time);
//
//        var valueDiff = higher.value - lower.value;
//
//        return lower.value + factor * valueDiff;
    }

}

[Serializable]
public struct InterpolationKeyFrame
{
    public float time;
    public float value;
    public float inTangent;
    public float outTangent;
    public int tangentMode;

    public InterpolationKeyFrame(Keyframe key)
    {
        time = key.time;
        value = key.value;
        inTangent = key.inTangent;
        outTangent = key.outTangent;
        tangentMode = key.tangentMode;
    }

    public Keyframe GetKeyFrame()
    {
        var key = new Keyframe(time, value, inTangent, outTangent);
        key.tangentMode = tangentMode;
        return key;
    }
}
