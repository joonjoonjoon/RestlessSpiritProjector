using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HudFPS : MonoBehaviour
{


    // Attach this to a GUIText to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.

    //edited by joon
	
    public  float updateInterval = 0.5F;
    public int cache;
    public Vector2 graphPosition;
    public Vector2 graphSize;
    public Color graphColor;
    public bool show;
    //private Camera graphCamera;
    private Text fpsText;
    private float accum = 0;
    // FPS accumulated over the interval
    private int frames = 0;
    // Frames drawn over the interval
    private float timeleft;
    // Left time for current interval
    private string color = "white";
    private FpsGraph fpsGraph;

    void Start()
    {
        fpsText = GetComponent<Text>();
        timeleft = updateInterval;

        // if (graphCamera == null)
        fpsGraph = Camera.main.gameObject.AddComponent<FpsGraph>();
//        else
//            fpsGraph = graphCamera.gameObject.AddComponent<FpsGraph>();
//        
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
		
        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2}", fps);
            fpsText.text = "<color=" + color + ">" + format + "</color>";
			
            if (fps < 30)
                color = "#ffff00";
				//guiText.material.color = Color.yellow;
			else if (fps < 10)
                color = "#ff0000";
            else
                color = "#00ff00";
            //	DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;

            fpsGraph.vals.Enqueue(fps);
            if (fpsGraph.vals.Count > fpsGraph.cache)
            {
                fpsGraph.vals.Dequeue();
            }


            //
            fpsGraph.xOffset = graphPosition.x;
            fpsGraph.yOffset = graphPosition.y;
            fpsGraph.width = graphSize.x;
            fpsGraph.height = graphSize.y;
            fpsGraph.color = graphColor;
            fpsGraph.cache = cache;
            fpsGraph.show = show;
        }
    }


    void OnDestroy()
    {
        Destroy(fpsGraph);
    }
}