using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshCombineTest : MonoBehaviour {
    public string outputname;
    public float threshold;
    public float bucketstep;
    public bool disableOriginalObjects;
    public bool disableOriginalMeshRenderers;
    public bool destroyOriginalObjects;
    public bool destroySCript;
	// Use this for initialization
	void Start () {

        outputname = transform.parent.name + gameObject.name;


        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            if (meshFilters[i].transform == transform)
            {
                i++;
                continue;
            }
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            if (disableOriginalObjects) meshFilters[i].gameObject.SetActive(false);
            if (disableOriginalMeshRenderers) meshFilters[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
            if (destroyOriginalObjects) Destroy(meshFilters[i].gameObject);
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;
        //AutoWeld(transform.GetComponent<MeshFilter>().mesh, threshold, bucketstep);
        //RemoveInternalTriangles(transform.GetComponent<MeshFilter>().mesh);
        //this.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;

        /*
        if(transform.name=="Outer")
            transform.parent.GetComponent<invertButtonStyle>().outerObject = gameObject;
        if (transform.name == "Inner")
        {
            transform.parent.GetComponent<invertButtonStyle>().innerObject = gameObject;
            Destroy(transform.parent.GetComponent<CreateOutlinedButton>());
        }
        */
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(transform.GetComponent<MeshFilter>().mesh, "Assets/Prefabs/ButtonModels/Meshes/combined_" + outputname + ".asset");
        UnityEditor.AssetDatabase.SaveAssets();

        transform.GetComponent<MeshFilter>().mesh = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefabs/ButtonModels/Meshes/combined_" + outputname + ".asset", typeof(Mesh)) as Mesh;
        Debug.Log(transform.GetComponent<MeshFilter>().mesh);
#endif

        if (destroySCript) Destroy(this);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void AutoWeld(Mesh mesh, float threshold, float bucketStep)
    {
        Vector3[] oldVertices = mesh.vertices;
        Vector3[] newVertices = new Vector3[oldVertices.Length];
        int[] old2new = new int[oldVertices.Length];
        int newSize = 0;

        // Find AABB
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int i = 0; i < oldVertices.Length; i++)
        {
            if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
            if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
            if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
            if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
            if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
            if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
        }

        // Make cubic buckets, each with dimensions "bucketStep"
        int bucketSizeX = Mathf.FloorToInt((max.x - min.x) / bucketStep) + 1;
        int bucketSizeY = Mathf.FloorToInt((max.y - min.y) / bucketStep) + 1;
        int bucketSizeZ = Mathf.FloorToInt((max.z - min.z) / bucketStep) + 1;
        List<int>[, ,] buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

        // Make new vertices
        for (int i = 0; i < oldVertices.Length; i++)
        {
            // Determine which bucket it belongs to
            int x = Mathf.FloorToInt((oldVertices[i].x - min.x) / bucketStep);
            int y = Mathf.FloorToInt((oldVertices[i].y - min.y) / bucketStep);
            int z = Mathf.FloorToInt((oldVertices[i].z - min.z) / bucketStep);

            // Check to see if it's already been added
            if (buckets[x, y, z] == null)
                buckets[x, y, z] = new List<int>(); // Make buckets lazily

            for (int j = 0; j < buckets[x, y, z].Count; j++)
            {
                Vector3 to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
                if (Vector3.SqrMagnitude(to) < threshold)
                {
                    old2new[i] = buckets[x, y, z][j];
                    goto skip; // Skip to next old vertex if this one is already there
                }
            }

            // Add new vertex
            newVertices[newSize] = oldVertices[i];
            buckets[x, y, z].Add(newSize);
            old2new[i] = newSize;
            newSize++;

        skip: ;
        }

        // Make new triangles
        int[] oldTris = mesh.triangles;
        int[] newTris = new int[oldTris.Length];
        for (int i = 0; i < oldTris.Length; i++)
        {
            newTris[i] = old2new[oldTris[i]];
        }

        Vector3[] finalVertices = new Vector3[newSize];
        for (int i = 0; i < newSize; i++)
            finalVertices[i] = newVertices[i];

        mesh.Clear();
        mesh.vertices = finalVertices;
        mesh.triangles = newTris;
        mesh.RecalculateNormals();
        ;
    }

    public void RemoveInternalTriangles(Mesh mesh)
    {
        var tris = mesh.GetTriangles(0);
        //var normals = mesh.normals;
        var internaltris = 0;
        Debug.Log(tris.Length + " " + mesh.subMeshCount);

        var newTris = new List<int>();
        for (int i = 0; i < tris.Length; i+=3)
        {
            for (int j = 0; j < tris.Length; j+=3)
            {
                if (i == j) continue;

                var iList = new List<int>() { tris[i], tris[i+1], tris[i+2] };
                var jList = new List<int>() { tris[j], tris[j + 1], tris[j + 2] };

                iList.Sort();
                jList.Sort();

                var count = 0;
                for (int k = 0; k < 3; k++)
                {
                    if(iList[k] == jList[k])
                    {
                        count++;
                    }
                }

                if (count == 3)
                {
                    internaltris++;
                }
                else
                {
                    newTris.AddRange(new List<int>(){tris[i], tris[i+1], tris[i+2]});
                }
            }
        }

        Debug.Log("Deleted " + internaltris + " internal tris");

        var verts = mesh.vertices;
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = newTris.ToArray();
        mesh.RecalculateNormals();
        ;
        
    }
}
