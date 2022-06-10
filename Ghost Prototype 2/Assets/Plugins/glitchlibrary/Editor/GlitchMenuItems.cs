using UnityEngine;
using UnityEditor;
using System.Collections;
using qtools.qhierarchy;
using System.Linq;
using UnityEditor.SceneManagement;

public class GlitchMenuItems
{
    [MenuItem("GameObject/GlitchLibrary/Sort Children Alphabetically",false, 0)]
    static void SortChildren() 
    {
        var sel = Selection.activeGameObject;
        if (sel == null) return;

        for (int j = 0; j < sel.transform.childCount; j++)
        {
            for (int i = 0; i < sel.transform.childCount - 1; i++)
            {
                if (string.Compare(sel.transform.GetChild(i).name, sel.transform.GetChild(i + 1).name) > 0)
                {
                    sel.transform.GetChild(i + 1).SetSiblingIndex(i);
                }
            }
        }
    }
}
