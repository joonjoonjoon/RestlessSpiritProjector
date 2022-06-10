using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CanEditMultipleObjects]
[CustomEditor(typeof(GlitchPrefab))]
public class GlitchPrefabEditor : Editor {

    private bool dirty;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!Application.isPlaying)
        {



            if (GUILayout.Button("Create instance (" + targets.Length + ")"))
            {
                foreach (var item in targets)
                {

                    var gp = (item as GlitchPrefab);
                    gp.Instantiate();
                }
            }
            if ((targets.Length > 1 || (target as GlitchPrefab).lastInstance != null) && GUILayout.Button("Remove instance (" + targets.Length + ")"))
            {
                foreach (var item in targets)
                {
                    var gp = (item as GlitchPrefab);
                    gp.DestroyLastInstance();
                }
            }
            if (GUILayout.Button("Remove all children (" + targets.Length + ")"))
            {
                foreach (var item in targets)
                {
                    var gp = (item as GlitchPrefab);
                    gp.DestroyAllChildren();
                }
            }
        }
    }
}
