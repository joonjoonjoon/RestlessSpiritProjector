using UnityEngine;
using System.Collections;

public class FindMissingScriptsUpdate : MonoBehaviour {

    static int go_count = 0, components_count = 0, missing_count = 0;

    void Awake()
    {
        FindInAll();
    }
	// Use this for initialization
	void Start () {
        FindInAll();
	}
	
	// Update is called once per frame
	void Update () {
        FindInAll();
	}

    private static void FindInAll()
    {
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        FindInGO();
        
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }


    private static void FindInGO()
    {

        GameObject[] obj = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject g in obj)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.Log(s + " has an empty script attached in position: " + i, g);
                }
            }
        }
    }
}
