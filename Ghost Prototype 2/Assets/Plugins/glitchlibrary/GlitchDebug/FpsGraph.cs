using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class FpsGraph : MonoBehaviour {

    public Queue<float> vals;
    public Material mat;
    public int cache = 10;
    public float xOffset;
    public float yOffset;
    public float width;
    public float height;
    public bool show;
    public Color color;

    // Use this for initialization
	void Awake () {
        vals = new Queue<float>();
        mat = new Material(Shader.Find("GUI/Text Shader"));
	}

    void OnPostRender()
    {
        if (!show) return;

        var scaleX = width / cache;
        var scaleY = height / 80;

        GL.PushMatrix();
        mat.SetPass(0);
        //GL.LoadPixelMatrix();
        GL.LoadPixelMatrix();        
        GL.Begin(GL.QUADS);

        GL.Color(color);
        var count = vals.Count();
        var v = vals.ToList();
        for (int i = 0; i < count; i++)
        {
            //if(i <)
            GL.Vertex3(Screen.width - xOffset + i * scaleX, Screen.height-yOffset + 0, 0);
            GL.Vertex3(Screen.width - xOffset + i * scaleX, Screen.height-yOffset + v[i] * scaleY, 0);
            GL.Vertex3(Screen.width - xOffset + (i + 1) * scaleX, Screen.height-yOffset + v[i] * scaleY, 0);
            GL.Vertex3(Screen.width - xOffset + (i + 1) * scaleX, Screen.height - yOffset + 0, 0);
        }
        
        GL.Color(Color.white);
        // 60 line
        GL.Vertex3(Screen.width - xOffset, Screen.height - yOffset + 30 * scaleY, 0);
        GL.Vertex3(Screen.width - xOffset + cache * scaleX, Screen.height - yOffset + (30) * scaleY, 0);
        GL.Vertex3(Screen.width - xOffset + cache * scaleX, Screen.height - yOffset + (31) * scaleY, 0);
        GL.Vertex3(Screen.width - xOffset, Screen.height - yOffset + 31 * scaleY, 0);
        // 30 line
        GL.Vertex3(Screen.width - xOffset, Screen.height - yOffset + 60 * scaleY, 0);
        GL.Vertex3(Screen.width - xOffset + cache * scaleX, Screen.height - yOffset + (60) * scaleY, 0);
        GL.Vertex3(Screen.width - xOffset + cache * scaleX, Screen.height - yOffset + (61) * scaleY, 0);
        GL.Vertex3(Screen.width - xOffset, Screen.height - yOffset + 61 * scaleY, 0);

        GL.End();
        GL.PopMatrix();
    }
}
