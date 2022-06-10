using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(Separator))]
public class SeparatorPropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        Color c = GUI.color;
        GUI.color = Color.white.SetA(0.6f);
        position.x -= 10;
        position.y += 10;
        EditorGUI.LabelField(position, "► " + label.text + " ", EditorStyles.boldLabel);
        GUI.color = c;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 25;
    }
}
