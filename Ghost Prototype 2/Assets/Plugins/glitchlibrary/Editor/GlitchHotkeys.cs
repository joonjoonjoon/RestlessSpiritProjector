using UnityEngine;
using UnityEditor;
using System.Collections;
using qtools.qhierarchy;
using System.Linq;
using UnityEditor.SceneManagement;

public class GlitchHotkeys
{
    [MenuItem("GlitchLibrary/Toggle Selected GameObject &LEFT")]
    static void EnableDisable()
    {
        if (Selection.gameObjects.Length > 0)
        {
            var scene = EditorSceneManager.GetActiveScene();
            var objectList = QHierarchy.createObjectListInScene(scene);
            QHierarchy.setVisibility(objectList, Selection.gameObjects.ToList(), !Selection.gameObjects[0].activeSelf, Selection.gameObjects[0].activeSelf);
        }
    }

    [MenuItem("GlitchLibrary/Toggle Selected GameObject alt &RIGHT")]
    static void EnableDisable2()
    {
        if (Selection.gameObjects.Length > 0)
        {
            var scene = EditorSceneManager.GetActiveScene();
            var objectList = QHierarchy.createObjectListInScene(scene);
            QHierarchy.setVisibility(objectList, Selection.gameObjects.ToList(), !Selection.gameObjects[0].activeSelf, !Selection.gameObjects[0].activeSelf);
        }
    }
}
