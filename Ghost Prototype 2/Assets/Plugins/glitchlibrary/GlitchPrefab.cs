using UnityEngine;
using System.Collections;

[ScriptOrder(-300)]
public class GlitchPrefab : MonoBehaviour
{

    public GameObject prefab;
    [HideInInspector]
    public GameObject lastInstance;
    public bool triggerOnAwakeIfHasFirstChild = true;
    public bool disableFirstChild = true;
    public bool destroyFirstChild = true;
    public bool disableScriptAfterRun = true;
    public bool destroyScriptAfterRun = true;
    public bool hasHappened = false;
    public bool destroyPrefabInstance = true;
    public bool inheritLayer = true;

    int count = 0;

    void Awake()
    {
        if (!hasHappened)
        {
            if (triggerOnAwakeIfHasFirstChild || transform.childCount == 0)
            {
                hasHappened = true;
                Instantiate();
            }
        }
    }

    public GameObject Instantiate()
    {
        if (disableFirstChild && transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(false);
        if (destroyFirstChild && transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);
        if (prefab == null)
            return null;
        GameObject go = null;

        if (Application.isPlaying)
        {
            go = Instantiate(prefab) as GameObject;
        }
        else
        {
#if UNITY_EDITOR
            

            if (UnityEditor.PrefabUtility.GetPrefabType(prefab) == UnityEditor.PrefabType.Prefab)
            {
                go = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            }
            else
            {
                go = Instantiate(prefab) as GameObject;
            }
#endif
        }

        go.transform.SetParent(transform);

        go.transform.localPosition = prefab.transform.localPosition;
        go.transform.localRotation = prefab.transform.localRotation;
        go.transform.localScale = prefab.transform.localScale;
        go.name = go.name.Replace("(Clone)", "");

        count++;

        var t = transform;
        while (t.parent != null)
        {
            t = t.parent;
        }

        if (Application.isPlaying && disableScriptAfterRun)
            this.enabled = false;
        if (Application.isPlaying && destroyScriptAfterRun)
        {
            Destroy(this);
        }

        if (destroyPrefabInstance)
        {
#if UNITY_EDITOR
            UnityEditor.PrefabUtility.DisconnectPrefabInstance(go);
#endif
        }

        if (inheritLayer)
        {
            go.SetLayer(gameObject.layer);
        }

        return go;
    }

    public void DestroyLastInstance()
    {
#if UNITY_EDITOR
        DestroyImmediate(lastInstance);
#else 
        Destroy(lastInstance);
#endif
    }

    public void DestroyAllChildren()
    {
        transform.Clear();
        DestroyLastInstance();
    }


    #if UNITY_EDITOR
    [UnityEditor.MenuItem("GlitchLibrary/GlitchPrefab Refresh All")]
    static void RefreshAll()
    {
        #if UNITY_5_3_3

        int counter = 0;
        foreach (var root in  UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var glitchPrefab in root.GetComponentsInChildren<GlitchPrefab>(true))
            {
                glitchPrefab.DestroyLastInstance();
                glitchPrefab.Instantiate();
                counter++;
            }

        }

        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
        Debug.Log("Refreshed " + counter + " GlitchPrefabs.");

        #else
        Debug.LogError("Not on version 5.3.3");
        #endif
    }

    [UnityEditor.MenuItem("GlitchLibrary/GlitchPrefab Refresh All + Destroy All Children")]
    static void RefreshAllAndDestroyChildren()
    {

//        #if UNITY_5_3_3

        int counter = 0;
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var glitchPrefab in root.GetComponentsInChildren<GlitchPrefab>(true))
            {
                glitchPrefab.DestroyAllChildren();
                glitchPrefab.Instantiate();
                counter++;
            }

        }

        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
        Debug.Log("Refreshed " + counter + " GlitchPrefabs.");
       // #else
        //Debug.LogError("Not on version 5.3.3");
       // #endif
    }
    #endif

}
