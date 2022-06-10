using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class GlitchDebugGraphItem
{
    public Queue<float> queue;
    public Color color;
    public float max;
    public bool includeNegative;
    public bool dot;
    public int cacheSize;
}

[RequireComponent(typeof(Camera))]
public class GlitchDebugGraph : MonoBehaviour {

    public static GlitchDebugGraph instance;
    public static void Add(string key, float val, int cacheSize, Color color, float max = 1, bool includeNegative = false, bool dot=false )
    {
        if (instance == null) Init();
        instance.Enqueue(key, val, cacheSize, color, max, includeNegative, dot);
    }

    private static void Init()
    {
        GameObject go = new GameObject("GlitchDebugGraphs");
        var cam = go.AddComponent<Camera>();
        instance = go.AddComponent<GlitchDebugGraph>();
        cam.depth = 1000;
    }

    public Dictionary<string,GlitchDebugGraphItem> vals;
    public Material mat;
    public float xOffset = 0.05f;
    public float yOffset = 0.25f;
    public float width = 0.9f;
    public float height = 0.5f;
    public bool show = true;

    
    const float minWidth = 1;

    // Use this for initialization
	void Awake () {
        vals = new Dictionary<string, GlitchDebugGraphItem>();
        mat = new Material(Shader.Find("GUI/Text Shader"));
        instance = this;

        Camera cam = GetComponent<Camera>();
        cam.cullingMask = 0;
        cam.depth = 100;
        cam.clearFlags = CameraClearFlags.Nothing;
	}

    private void Enqueue(string key, float val, int cacheSize, Color color, float max = 1, bool includeNegative = false, bool dot=false )
    {
        if (!vals.ContainsKey(key))
        {
            vals.Add(key, new GlitchDebugGraphItem());
            vals[key].queue= new Queue<float>();
            vals[key].dot= dot;
            vals[key].color = color;
            vals[key].max = max;
            vals[key].cacheSize = cacheSize;
            vals[key].includeNegative = includeNegative;
        }

        vals[key].queue.Enqueue(val);
        if (vals[key].queue.Count > vals[key].cacheSize)
        {
            vals[key].queue.Dequeue();
        }
    }

    void OnPostRender()
    {
        if (!show) return;

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadPixelMatrix();        
        GL.Begin(GL.QUADS);

        foreach (GlitchDebugGraphItem item in vals.Values)
        {
            var scaleX = (width * Screen.width) / item.cacheSize;
            var scaleY = (height * Screen.height) / item.max;
            var sliceWidth = Mathf.Max(scaleX,minWidth);
            var currentXOffset = xOffset * Screen.width;
            var currentYOffset = yOffset * Screen.height;

            if(item.includeNegative) 
            {
                scaleY = scaleY / 2f;
                currentYOffset += (height * Screen.height) / 2;
            }

            GL.Color(item.color);
            var queueAsList = item.queue.ToList();
            for (int i = 0; i < queueAsList.Count; i++)
            {
                GL.Vertex3(currentXOffset + i * scaleX, currentYOffset + (item.dot ? (queueAsList[i] * scaleY) + 2 : 0), 0);
                GL.Vertex3(currentXOffset + i * scaleX, currentYOffset + queueAsList[i] * scaleY, 0);
                GL.Vertex3(currentXOffset + i * scaleX + sliceWidth, currentYOffset + queueAsList[i] * scaleY, 0);
                GL.Vertex3(currentXOffset + i * scaleX + sliceWidth, currentYOffset + (item.dot ? (queueAsList[i] * scaleY) + 2 : 0), 0);
            }

            /*
            GL.Color(Color.white);
            // 60 line
            GL.Vertex3(xOffset, yOffset + 30 * scaleY, 0);
            GL.Vertex3(xOffset + cache * scaleX, yOffset + (30) * scaleY, 0);
            GL.Vertex3(xOffset + cache * scaleX, yOffset + (31) * scaleY, 0);
            GL.Vertex3(xOffset, yOffset + 31 * scaleY, 0);
            // 30 line
            GL.Vertex3(xOffset, yOffset + 60 * scaleY, 0);
            GL.Vertex3(xOffset + cache * scaleX, yOffset + (60) * scaleY, 0);
            GL.Vertex3(xOffset + cache * scaleX, yOffset + (61) * scaleY, 0);
            GL.Vertex3(xOffset, yOffset + 61 * scaleY, 0);
            */
        }

        GL.End();
        GL.PopMatrix();

    }
}
