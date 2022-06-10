using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DoToAllTags))]
public class DoToAllTagsEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("DO SORTING"))
        {
            (target as DoToAllTags).SetAllSortingLayers();
        }
        if (GUILayout.Button("DO APPLY"))
        {
            (target as DoToAllTags).ApplyPrefabsWithTag();
        }
    }
}
