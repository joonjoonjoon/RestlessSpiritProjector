using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraGizmo2D : MonoBehaviour {
    public bool aspect_current;
    public bool aspect_16_9;
    public bool aspect_4_3;
    public bool aspect_9_16;
    public bool aspect_3_4;
    public float[] guidesX;
    public float[] guidesY;
    
    void OnDrawGizmos()
    {
        Camera cam = GetComponent<Camera>();
        
        if (aspect_current) DrawAspect(cam, cam.pixelWidth, cam.pixelHeight, Color.green, true);
        if (aspect_9_16) DrawAspect(cam, 9, 16, Color.yellow);
        if (aspect_4_3) DrawAspect(cam, 4, 3, Color.cyan);
        if (aspect_3_4) DrawAspect(cam, 3, 4, ColorExtension.hexToColor("5468FF"));
        if (aspect_16_9) DrawAspect(cam, 16, 9, Color.red);

        Gizmos.color = Color.cyan.SetA(0.4f);
        if (guidesX != null)
        {
            foreach (float x in guidesX)
            {
                Gizmos.DrawLine(transform.position + new Vector3(x, -100, 0), transform.position + new Vector3(x, 100, 0));
            }
        }
        if (guidesY != null)
        {
            foreach (float y in guidesY)
            {
                Gizmos.DrawLine(transform.position + new Vector3(-100, y, 0), transform.position + new Vector3(100, y, 0));
            }
        }
    }

    void DrawAspect(Camera cam, float h, float v, Color color, bool push=false)
    {
        Gizmos.color = color;
        float ratiowidth = (cam.pixelHeight / v * h);
        float x = (cam.pixelWidth / 2f) - (ratiowidth / 2f);

        Vector3 point1 = cam.ScreenToWorldPoint(new Vector3(x, 0, 0));
        Vector3 point2 = cam.ScreenToWorldPoint(new Vector3(x, cam.pixelHeight, 0));
        Vector3 point3 = cam.ScreenToWorldPoint(new Vector3(x + ratiowidth, cam.pixelHeight, 0));
        Vector3 point4 = cam.ScreenToWorldPoint(new Vector3(x + ratiowidth, 0, 0));

        GizmosExtension.DrawPoly(point1, point2, point3, point4);

#if UNITY_EDITOR        
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;        
        UnityEditor.Handles.Label(point4, (push?"\n":"")+h + ":" + v , style);
#endif
    }
}
