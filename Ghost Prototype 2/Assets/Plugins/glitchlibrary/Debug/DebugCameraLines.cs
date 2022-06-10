using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugCameraLines : MonoBehaviour
{
    private static DebugCameraLines instance;

    public static void AddLine(Vector3 v1, Vector3 v2)
    {
        AddLine(v1, v2, Color.white);
    }

    public static void AddLine(Vector3 v1, Vector3 v2, Color color)
    {
        if (instance == null)
        {
            instance = Camera.main.gameObject.AddComponent<DebugCameraLines>();
        }
        instance.addLine(v1, v2, color);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            clear();
        }
    }

    public static void Clear()
    {
        if (instance == null)
        {
            instance = Camera.main.gameObject.AddComponent<DebugCameraLines>();
        }
        instance.clear();
    }

    private List<Vector3> starts;
    private List<Vector3> ends;
    private List<Color> colors;
    private Material mat;
    
    void Start()
    {
        mat = new Material(Shader.Find("GUI/Text Shader"));
    }

    public void addLine(Vector3 v1, Vector3 v2, Color color)
    {
        if (starts == null) starts = new List<Vector3>();
        if (ends == null) ends = new List<Vector3>();
        if (colors == null) colors = new List<Color>();
        starts.Add(v1);
        ends.Add(v2);
        colors.Add(color);
    }

    public void clear()
    {
        starts = null;
        colors = null;
        ends = null;
    }

    void OnPostRender()
    {
        if (starts != null && ends != null && colors != null)
        {
            GL.PushMatrix();
            mat.SetPass(0);
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINES);

            for (int i = 0; i < starts.Count; i++)
            {
                GL.Color(colors[i]);
                GL.Vertex(starts[i]);
                GL.Vertex(ends[i]);
            }
            
            GL.End();
            GL.PopMatrix();
        }
    }
}