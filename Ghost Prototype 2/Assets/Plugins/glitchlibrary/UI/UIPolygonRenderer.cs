using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
public class UIPolygonRenderer : MaskableGraphic
{

    [SerializeField]
    Texture m_Texture;
    [SerializeField]
    Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

    //public float LineThickness = 2;
    public bool UseMargins;
    public Vector2 Margin;
    public Vector2[] Points;
    public bool relativeSize;
    public float capSize = 12;

    public override Texture mainTexture
    {
        get
        {
            return m_Texture == null ? s_WhiteTexture : m_Texture;
        }
    }

    /// <summary>
    /// Texture to be used.
    /// </summary>
    public Texture texture
    {
        get
        {
            return m_Texture;
        }
        set
        {
            if (m_Texture == value)
                return;

            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    /// <summary>
    /// UV rectangle used by the texture.
    /// </summary>
    public Rect uvRect
    {
        get
        {
            return m_UVRect;
        }
        set
        {
            if (m_UVRect == value)
                return;
            m_UVRect = value;
            SetVerticesDirty();
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        // requires sets of quads
//        if (Points == null || Points.Length < 4 || (Points.Length % 2 != 0))
//            Points = new[] { new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0) };
//
//        var sizeX = rectTransform.rect.width;
//        var sizeY = rectTransform.rect.height;
//        var offsetX = -rectTransform.pivot.x * rectTransform.rect.width;
//        var offsetY = -rectTransform.pivot.y * rectTransform.rect.height;
//
//        // don't want to scale based on the size of the rect, so this is switchable now
//        if (!relativeSize)
//        {
//            sizeX = 1;
//            sizeY = 1;
//        }
//        // build a new set of points taking into account the cap sizes. 
//        // would be cool to support corners too, but that might be a bit tough :)
//        var pointList = new List<Vector2>();
//        pointList.Add(Points[0]);
//        var capPoint = Points[0] + (Points[1] - Points[0]).normalized * capSize;
//        pointList.Add(capPoint);
//
//        // should bail before the last point to add another cap point
//        for (int i = 1; i < Points.Length - 1; i++)
//        {
//            pointList.Add(Points[i]);
//        }
//        capPoint = Points[Points.Length - 1] - (Points[Points.Length - 1] - Points[Points.Length - 2]).normalized * capSize;
//        pointList.Add(capPoint);
//        pointList.Add(Points[Points.Length - 1]);
//
//        var TempPoints = pointList.ToArray();
//        if (UseMargins)
//        {
//            sizeX -= Margin.x;
//            sizeY -= Margin.y;
//            offsetX += Margin.x / 2f;
//            offsetY += Margin.y / 2f;
//        }

        vh.Clear();

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
            

        foreach (var v in vertices)
        {
            var vert = UIVertex.simpleVert;
            vert.color = color;
            vert.position = v;
            vert.uv0 = Vector3.zero;
            vh.AddVert(vert);
        }

        for (int i = 0; i < indices.Length; i += 3)
        {
            vh.AddTriangle(indices[i], indices[i + 1], indices[i + 2]);
        }
            
//        // Create the mesh
//        Mesh msh = new Mesh();
//        msh.vertices = vertices;
//        msh.triangles = indices;
//        msh.RecalculateNormals();
//        msh.RecalculateBounds();
//
//        // Set up game object with mesh;
//        GetComponent<MeshFilter>().mesh = msh;
//
//        Vector2 prevV1 = Vector2.zero;
//        Vector2 prevV2 = Vector2.zero;
//
//        for (int i = 2; i < TempPoints.Length; i += 2)
//        {
//            var prevUpper = TempPoints[i - 2];
//            var prevLower = TempPoints[i - 1];
//            var curLower = TempPoints[i];
//            var curUpper = TempPoints[i + 1];
//            prevLower = new Vector2(prevLower.x * sizeX + offsetX, prevLower.y * sizeY + offsetY);
//            prevUpper = new Vector2(prevLower.x * sizeX + offsetX, prevLower.y * sizeY + offsetY);
//            curUpper = new Vector2(curUpper.x * sizeX + offsetX, curUpper.y * sizeY + offsetY);
//            curLower = new Vector2(curUpper.x * sizeX + offsetX, curUpper.y * sizeY + offsetY);
//
//            // float angle = Mathf.Atan2(curUpper.y - prevUpper.y, curUpper.x - prevUpper.x) * 180f / Mathf.PI;
//
//            var v1 = prevUpper;
//            var v2 = prevLower;
//            var v3 = curLower;
//            var v4 = curUpper;
//
////            v1 = RotatePointAroundPivot(v1, prevLower, new Vector3(0, 0, angle));
////            v2 = RotatePointAroundPivot(v2, prevUpper, new Vector3(0, 0, angle));
////            v3 = RotatePointAroundPivot(v3, curUpper, new Vector3(0, 0, angle));
////            v4 = RotatePointAroundPivot(v4, curLower, new Vector3(0, 0, angle));
//
//            Vector2 uvTopLeft = Vector2.zero;
//            Vector2 uvBottomLeft = new Vector2(0, 1);
//
//            Vector2 uvTopCenter = new Vector2(0.5f, 0);
//            Vector2 uvBottomCenter = new Vector2(0.5f, 1);
//
//            Vector2 uvTopRight = new Vector2(1, 0);
//            Vector2 uvBottomRight = new Vector2(1, 1);
//
//            Vector2[] uvs = new[] { uvTopCenter, uvBottomCenter, uvBottomCenter, uvTopCenter };
//
//            if (i > 1)
//            {
//                vh.AddUIVertexQuad(SetVbo(new[] { prevV1, prevV2, v1, v2 }, uvs));
//            }
//
//            if (i == 2)
//            {
//                uvs = new[] { uvTopLeft, uvBottomLeft, uvBottomCenter, uvTopCenter };
//            }
//            else if (i == TempPoints.Length - 2)
//            {
//                uvs = new[] { uvTopCenter, uvBottomCenter, uvBottomRight, uvTopRight };
//            }
//
//            vh.AddUIVertexQuad(SetVbo(new[] { v1, v2, v3, v4 }, uvs));
//
//
//            prevV1 = v3;
//            prevV2 = v4;
//        }
    }

    protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
    {
        UIVertex[] vbo = new UIVertex[4];
        for (int i = 0; i < vertices.Length; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.color = color;
            vert.position = vertices[i];
            vert.uv0 = uvs[i];
            vbo[i] = vert;
        }
        return vbo;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    //    public bool UpdateRealtime;

    //    void Start()
    //    {
    //        if (Application.isPlaying)
    //        {
    //            DoUpdate();
    //            Destroy(GetComponent<PolygonCollider2D>());
    //        }
    //        else
    //        {
    //            DoUpdate();
    //            if (GetComponent<PolygonCollider2D>() == null)
    //                gameObject.AddComponent<PolygonCollider2D>();
    //            if (GetComponent<MeshFilter>() == null)
    //                gameObject.AddComponent<MeshFilter>();
    //            if (GetComponent<MeshRenderer>() == null)
    //                gameObject.AddComponent<MeshRenderer>();
    //            /*if(GetComponent<MeshEditor>()!=null)
    //            {
    //                var verts = new List<Vector2>();
    //                for (int i = 0; i < transform.childCount; i++)
    //                {
    //                    var x = i;
    //                    if (i == 3) continue;
    //                    if (x == 2) x = 4;
    //                    else if (x == 4) x = 2;
    //                    var item = transform.GetChild(x).localPosition;
    //                    verts.Add(new Vector2(item.x, item.y));
    //
    //                }
    //                GetComponent<PolygonCollider2D>().points = verts.ToArray();
    //
    //                foreach (Transform item in transform)
    //                {
    //                    DestroyImmediate(item.gameObject);
    //                }
    //            }*/
    //        }
    //    }

    //    void DoUpdate()
    //    {
    //        // Create Vector2 vertices
    //        Vector2[] vertices2D = GetComponent<PolygonCollider2D>().points;
    //
    //        // Use the triangulator to get indices for creating triangles
    //        Triangulator tr = new Triangulator(vertices2D);
    //        int[] indices = tr.Triangulate();
    //
    //        // Create the Vector3 vertices
    //        Vector3[] vertices = new Vector3[vertices2D.Length];
    //        for (int i = 0; i < vertices.Length; i++)
    //        {
    //            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
    //        }
    //
    //        // Create the mesh
    //        Mesh msh = new Mesh();
    //        msh.vertices = vertices;
    //        msh.triangles = indices;
    //        msh.RecalculateNormals();
    //        msh.RecalculateBounds();
    //
    //        // Set up game object with mesh;
    //        GetComponent<MeshFilter>().mesh = msh;
    //    }

    //    void Update()
    //    {
    //        if (!Application.isPlaying && UpdateRealtime)
    //        {
    //
    //            #if UNITY_EDITOR
    //            if (UnityEditor.Selection.Contains(gameObject))
    //                DoUpdate();
    //            #endif
    //
    //
    //        }
    //    }

    //    void OnDrawGizmosSelected()
    //    {
    //        Gizmos.DrawLine(transform.position - Vector3.up, transform.position + Vector3.up);
    //        Gizmos.DrawLine(transform.position - Vector3.left, transform.position + Vector3.left);
    //        Gizmos.DrawLine(transform.position - Vector3.forward, transform.position + Vector3.forward);
    //    }
}
