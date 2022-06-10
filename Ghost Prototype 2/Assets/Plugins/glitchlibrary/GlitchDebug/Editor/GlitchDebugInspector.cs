using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GlitchDebug))]
class GlitchDebugInspector : Editor {
	public override void OnInspectorGUI() {
		if(GUILayout.Button("Toggle Debugger"))
		{
			((GlitchDebug)target).transform.FindChild("InnerCanvas").GetComponent<Canvas>().enabled = !((GlitchDebug)target).transform.FindChild("InnerCanvas").GetComponent<Canvas>().enabled;
			((GlitchDebug)target).transform.FindChild("ShowToggle").GetComponent<Toggle>().isOn = !((GlitchDebug)target).transform.FindChild("ShowToggle").GetComponent<Toggle>().isOn;
		}
        if (GUILayout.Button("Toggle Black/White"))
        {
            ((GlitchDebug) target).ToggleBlackWhite();
        }
		DrawDefaultInspector();

	}
}