using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(InspectorShowIfAttribute))]
public class DrawerInspectorShowIf : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string showIf = (attribute as InspectorShowIfAttribute).showIf;

        var show = property.serializedObject.FindProperty(showIf).boolValue;

        if (show)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        string showIf = (attribute as InspectorShowIfAttribute).showIf;

        var show = property.serializedObject.FindProperty(showIf).boolValue;

        if (show)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
            
        return 0f;
    }
}
