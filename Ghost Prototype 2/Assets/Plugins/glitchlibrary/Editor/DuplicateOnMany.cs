using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class DuplicateOnMany : EditorWindow
{
    GameObject obj;
    System.Action action;
    GameObject[] selection;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Duplicate On Many")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DuplicateOnMany window = (DuplicateOnMany)EditorWindow.GetWindow(typeof(DuplicateOnMany));
        window.titleContent.text = "Duplicate On Many";

        window.Show();

        Selection.selectionChanged += window.SelectionChanged;
        window.action = Selection.selectionChanged;
    }


    void OnGUI()
    {
        if (Selection.selectionChanged != action)
        {
            Selection.selectionChanged += SelectionChanged;
            action = Selection.selectionChanged;
        }

        obj = (GameObject)EditorGUILayout.ObjectField((obj ? obj.name : "no selection"), obj, typeof(GameObject), true);

        if (selection == null || selection.Length == 0)
        {
            EditorGUILayout.LabelField("Select something...");
        }
        else
        {
            var selectionString = "";
            for (int i = 0; i < selection.Length; i++)
            {
                selectionString += selection[i].name + (i < selection.Length - 2 ? ", " : "");
            }

            GUILayout.Label("This will create " + (obj ? obj.name : "no selection") + " into " + selectionString, EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Do it."))
            {
                foreach (var sel in selection)
                {
                    if (sel == obj)
                    {
                        Debug.Log("Circular loop detected");
                        continue;
                    }

                    var newGo = Instantiate(obj) as GameObject;
                    newGo.transform.parent = sel.transform;
                    newGo.transform.localPosition = obj.transform.localPosition;
                    newGo.transform.localRotation = obj.transform.localRotation;
                    newGo.transform.localScale = obj.transform.localScale;
                    newGo.transform.name = newGo.transform.name.Replace("(Clone)", "");
                }
            }
        }

    }

    void SelectionChanged()
    {
        DebugExtension.Blip();

        var selectedObjs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Unfiltered);
        selection = System.Array.ConvertAll(selectedObjs, x => (GameObject)x);

        Repaint();
    }
}