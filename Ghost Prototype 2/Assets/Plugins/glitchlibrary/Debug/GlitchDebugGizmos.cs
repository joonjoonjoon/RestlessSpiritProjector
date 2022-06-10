using UnityEngine;
using System.Collections;

public class GlitchDebugGizmos : MonoBehaviour {

    public bool showSphere;
    public bool showWireSphere;
    public Color color = Color.green;
    public float scale=0.1f;


    
    void OnDrawGizmos()
    {
        Gizmos.color = color;
        if (showSphere) Gizmos.DrawSphere(transform.position, scale);
        if (showWireSphere) Gizmos.DrawWireSphere(transform.position, scale);

    }
    
}
