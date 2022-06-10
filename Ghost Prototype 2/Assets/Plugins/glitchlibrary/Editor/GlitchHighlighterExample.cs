using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class GlitchHighlighterExample {
/*
    static Texture2D texturePanel;

    static GlitchHighlighterExample()
    {
        texturePanel = AssetDatabase.LoadAssetAtPath ("Assets/glitchlibrary/Editor/images/GlitchPrefabIcon.png", typeof(Texture2D)) as Texture2D;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }
 
    static void HierarchyItemCB (int instanceID, Rect selectionRect)
    {
        // place the icon to the right of the list:
        Rect r = new Rect (selectionRect); 
        r.x = r.width - 20;
        r.width = 20;
 
        GameObject go = EditorUtility.InstanceIDToObject (instanceID) as GameObject;
        if (go != null && go.GetComponent<GlitchMissingLinkGuard>()) 
            GUI.Label (r, texturePanel);
    }
 */
}
