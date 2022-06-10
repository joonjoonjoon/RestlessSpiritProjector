using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(InspectorNoteAttribute))]
public class DrawerInspectorNote : PropertyDrawer
{
    /// <summary>
    /// Draw our editor
    /// </summary>
    /// <param name="position"></param>
    /// <param name="property"></param>
    /// <param name="label"></param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InspectorNoteAttribute note = attribute as InspectorNoteAttribute;

        var isBool = property.type == "bool";

        Color color = GUI.color;
        Color faded = note.color;

        GUI.color = faded;

        // our header is always present
        Rect posLabel = position;
        posLabel.y += 13;
        posLabel.x += 2;
        posLabel.height += 13;
        EditorGUI.LabelField(posLabel, note.header, EditorStyles.whiteLargeLabel);

        if (!isBool)
        {
            var height = string.IsNullOrEmpty(note.message) ? 38f : 50f;

            var labelHeight = EditorGUI.GetPropertyHeight(property, label, false);

            var propPos = new Rect(position);
            propPos.y += height - labelHeight;

            EditorGUI.PropertyField(propPos, property, GUIContent.none, true);
        }

        GUI.color = color;
        // do we have a message too?
        if (!string.IsNullOrEmpty(note.message))
        {
            Rect posExplain = posLabel;
            posExplain.y += 15;
            GUI.color = faded;
            EditorGUI.LabelField(posExplain, note.message, EditorStyles.whiteMiniLabel);
            GUI.color = color;
        }

        Rect posLine = position;
        posLine.y += string.IsNullOrEmpty(note.message) ? 34 : 45;
        //posLine.x += 15;;
        posLine.height = 2;
        
        GUI.Box(posLine, "");

        GUI.color = Color.black;
        posLine.y += 2;
        GUI.Box(posLine, "");
        GUI.color = color;
    }

    /// <summary>
    /// Override height in case of error
    /// </summary>
    /// <param name="prop"></param>
    /// <param name="label"></param>
    /// <returns></returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        InspectorNoteAttribute note = attribute as InspectorNoteAttribute;

        var height = string.IsNullOrEmpty(note.message) ? 38f : 50f;

        if (property.isExpanded)
        {
            height += EditorGUI.GetPropertyHeight(property, GUIContent.none, true);
            height -= EditorGUI.GetPropertyHeight(property, label, false);
        }

        return height;
    }
}
