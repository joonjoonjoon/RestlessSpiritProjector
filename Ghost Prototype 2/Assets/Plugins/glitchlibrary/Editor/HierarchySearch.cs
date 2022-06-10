using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class HierarchySearch : EditorWindow
{
#if UNITY_5_3_3
    public string searchstring;
    public string[] searchstringSplit;
    public Transform lastSelection;
    public bool previousFound;
    
    // Add menu named "My Window" to the Window menu
    [MenuItem("GlitchLibrary/HierarchySearch")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        HierarchySearch window = (HierarchySearch)EditorWindow.GetWindow(typeof(HierarchySearch));
        window.minSize = new Vector2(10, 10);
        window.titleContent.text = "Hierarchy Search";
        window.ShowUtility();
        window.searchstring = "";
    }


    void OnGUI()
    {
        var temp = searchstring;
        searchstring = EditorGUILayout.TextField("Filter", searchstring);
        if(searchstring != temp)
        {
            searchstringSplit = searchstring.Split(' ');
            lastSelection = null;
            Search();

        }

        Event e = Event.current;
        if(e.type == EventType.keyDown && e.keyCode == KeyCode.Tab)
        {
            Search();
        }
        if (e.type == EventType.keyDown && e.keyCode == KeyCode.Return)
        {
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            EditorWindow.GetWindow(type).Focus();
            Debug.Log("tried");
        }
    }

    void Search()
    {
        previousFound = false;
        bool found = false;
        //var timestart = EditorApplication.timeSinceStartup;
        var currentLastSelection = lastSelection;
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (SearchRecurse(root.transform))
            {
                found = true;
                break;
            }
        }
        //Debug.Log("Took " + GetTimeString(EditorApplication.timeSinceStartup - timestart));

        if(currentLastSelection == lastSelection && lastSelection != null)
        {
            lastSelection = null;
            Search();
        }
        else if (!found)
        {
            Selection.activeGameObject = null;
        }
    }

    bool SearchRecurse(Transform transformFolder)
    {
        bool contains = true;
        foreach (var ss in searchstringSplit)
        {
            contains &= transformFolder.name.IndexOf(ss, System.StringComparison.OrdinalIgnoreCase) >= 0;
            if (!contains) break;
        }
        
        if (contains)
        {
            if(transformFolder == lastSelection)
            {
                previousFound = true;
            }
            else
            {
                if (previousFound || lastSelection == null)
                {
                    Selection.activeGameObject = transformFolder.gameObject;
                    lastSelection = transformFolder;
                    return true;
                }
            }
            
        }

        foreach (Transform t in transformFolder)
        {
            if(SearchRecurse(t))
            {
                return true;
            }
        }
        return false;
    }


    private static string GetTimeString(double time)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(time);

        if (t.Minutes > 0)
            return string.Format("{0:D2}m:{1:D2}s", t.Minutes, t.Seconds);
        else
            return string.Format("{0:D2}s:{1:D3}ms", t.Seconds, t.Milliseconds);
    }
#endif
}
