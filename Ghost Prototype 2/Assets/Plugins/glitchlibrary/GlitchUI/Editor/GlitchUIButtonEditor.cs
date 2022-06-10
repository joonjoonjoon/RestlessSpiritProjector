using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlitchUIButton))]
[CanEditMultipleObjects]
public class GlitchUIButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        //var instance = (target as GlitchUIButton);
    }
   
}
