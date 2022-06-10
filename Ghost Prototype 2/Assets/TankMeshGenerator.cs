using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMeshGenerator : MonoSingleton<TankMeshGenerator> {
    


    public int segmentsX;
    public int segmentsY;
    public MeshFilter filter;
    public Transform[] points;

    public Vector3[] verts;
    public int[] tris;

    public bool LiveUpdate;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GenerateMesh();
    }

    void Update()
    {
        if (LiveUpdate) 
            GenerateMesh();
    }

    void GenerateMesh()
    {
        // find and clean filter
        if (filter.mesh != null) filter.mesh.Clear();

        // prep empty arrays
        Mesh mesh = new Mesh();
        //Vector3[] verts = new Vector3[segmentsX  * segmentsY];
        verts = new Vector3[(segmentsX +1) * (segmentsY +1)];
        Vector3[] norms = new Vector3[verts.Length];
        Vector2[] uvs = new Vector2[verts.Length];

        //int[] tris = new int[(segmentsX - 1) * (segmentsY -1) * 2 * 3];
        tris = new int[segmentsX * segmentsY * 2 * 3];
        var center = transform.position;

        for (int x = 0; x < segmentsX +1; x++)
        {
            for (int y = 0; y < segmentsY+1; y++)
            {
                var bottomX = Vector3.Lerp(points[0].position - center, points[3].position - center, x / (float)segmentsX);
                var topX = Vector3.Lerp(points[1].position - center, points[2].position - center, x / (float)segmentsX);

                verts[GetIndex(x, y)] = Vector3.Lerp(bottomX, topX, y / (float)segmentsY);

                if(x < segmentsX && y < segmentsY)
                {
                    var id = (x + y * segmentsX)  * 2 * 3;
                    tris[id + 0] = GetIndex(x, y);
                    tris[id + 2] = GetIndex(x+1, y);
                    tris[id + 1] = GetIndex(x, y+1);
                    id+=3;
                    tris[id + 0] = GetIndex(x+1, y);
                    tris[id + 2] = GetIndex(x + 1, y+1);
                    tris[id + 1] = GetIndex(x, y + 1);
                }

                uvs[GetIndex(x, y)] = new Vector2(x / (float)segmentsX, y / (float)segmentsY);
            }
        }

        mesh.vertices = verts;
        mesh.normals = norms;
        mesh.uv = uvs;
        mesh.triangles = tris;
        filter.mesh = mesh;


    }

    private int GetIndex(int x, int y)
    {
        return x + (y * (segmentsX+1));
    }



}




