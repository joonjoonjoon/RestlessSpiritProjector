using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class TriangulatePolygonCollider : MonoBehaviour {
    public bool UpdateRealtime;

    void Start()
    {
        if (Application.isPlaying)
        {
            DoUpdate();
            Destroy(GetComponent<PolygonCollider2D>());
        }
        else
        {
            DoUpdate();
            if (GetComponent<PolygonCollider2D>() == null) gameObject.AddComponent<PolygonCollider2D>();
			if (GetComponent<MeshFilter>() == null) gameObject.AddComponent<MeshFilter>();
			if (GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();
            /*if(GetComponent<MeshEditor>()!=null)
            {
                var verts = new List<Vector2>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    var x = i;
                    if (i == 3) continue;
                    if (x == 2) x = 4;
                    else if (x == 4) x = 2;
                    var item = transform.GetChild(x).localPosition;
                    verts.Add(new Vector2(item.x, item.y));

                }
                GetComponent<PolygonCollider2D>().points = verts.ToArray();

                foreach (Transform item in transform)
                {
                    DestroyImmediate(item.gameObject);
                }
            }*/
        }
    }
    void DoUpdate()
    {
        // Create Vector2 vertices
        Vector2[] vertices2D = GetComponent<PolygonCollider2D>().points;

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        GetComponent<MeshFilter>().mesh = msh;
    }

    void Update()
    {
        if (!Application.isPlaying && UpdateRealtime)
        {
        
#if UNITY_EDITOR
            if(UnityEditor.Selection.Contains(gameObject))
                DoUpdate();
#endif


        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position - Vector3.up, transform.position + Vector3.up);
        Gizmos.DrawLine(transform.position - Vector3.left, transform.position + Vector3.left);
        Gizmos.DrawLine(transform.position - Vector3.forward, transform.position + Vector3.forward);
    }
}
