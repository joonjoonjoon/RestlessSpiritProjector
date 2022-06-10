using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HoldDownButton))]
public class HoldDownButtonEditor: Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

}
