using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DoToAllTags : MonoBehaviour {

    public string taggg;
    public int sortorder;

    public void SetAllSortingLayers()
    {
        int count = 0;
        foreach (var item in GameObject.FindGameObjectsWithTag(taggg))
        {
            item.GetComponent<Renderer>().sortingOrder = sortorder;
            count++;
        }
        Debug.Log("Did it to " + count + " objects");
    }


    public void ApplyPrefabsWithTag()
    {
#if UNITY_EDITOR
        int count = 0;

        foreach (var item in GameObject.FindGameObjectsWithTag(taggg))
        {
            PrefabUtility.ReplacePrefab(item, PrefabUtility.GetPrefabParent(item), ReplacePrefabOptions.ConnectToPrefab);
            Debug.Log("Applied " + item.name);            count++;

        }
        Debug.Log("Did it to " + count + " objects");

#endif
    }


}
