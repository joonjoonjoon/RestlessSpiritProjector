using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CustomPropertyDrawer(typeof(InterpolationList))]
public class InterpolationListDrawer: PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {   
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject) as InterpolationList;        

        EditorGUI.PropertyField(position, property, label, true);

        var anim = new AnimationCurve(obj.list.Select(item => item.GetKeyFrame()).ToArray());

        anim = EditorGUI.CurveField(new Rect(position.x, position.y + position.height - 50f, position.width, 50f), anim);

        if (obj.list.Count != anim.length)
        {
            while (obj.list.Count < anim.length)
            {
                obj.list.Add(new InterpolationKeyFrame());
            }

            while (obj.list.Count > anim.length)
            {
                obj.list.RemoveAt(obj.list.Count - 1);
            }
        }

        for (int i = 0; i < anim.length; i++)
        {
            if (obj.list[i].time != anim.keys[i].time ||
                obj.list[i].value != anim.keys[i].value ||
                obj.list[i].inTangent != anim.keys[i].inTangent ||
                obj.list[i].outTangent != anim.keys[i].outTangent ||
                obj.list[i].tangentMode != anim.keys[i].tangentMode)
            {
                obj.list[i] = new InterpolationKeyFrame(anim.keys[i]);
            }
        }

        obj.UpdateInternalAnimationCurve(anim);

        fieldInfo.SetValue(property.serializedObject.targetObject, obj);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) + 50f;
    }


}
