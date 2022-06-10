using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class MassApplyFont : EditorWindow
{
    private TMPro.TMP_FontAsset font;

    [MenuItem("GlitchLibrary/Mass Apply Font")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        MassApplyFont window = (MassApplyFont)EditorWindow.GetWindow(typeof(MassApplyFont));
        window.titleContent.text = "Mass apply font";
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Font:", EditorStyles.boldLabel);
        font = (TMPro.TMP_FontAsset)EditorGUILayout.ObjectField("Label:", font, typeof(TMPro.TMP_FontAsset), true);

        if (GUILayout.Button("APPLY"))
        {
            ApplyFont();
        }

    }

    private void ApplyFont()
    {
        int i = 0;
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {

            foreach (var t in root.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
            {
                t.font = font as TMPro.TMP_FontAsset;
                i++;
            }
        }

        Debug.Log("Applied " + font.name + " to " + i + " texts.");
    }

}