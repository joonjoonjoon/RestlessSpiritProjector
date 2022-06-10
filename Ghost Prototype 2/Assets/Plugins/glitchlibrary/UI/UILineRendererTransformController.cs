using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILineRenderer))]
[ExecuteInEditMode]
public class UILineRendererTransformController : MonoBehaviour {
    public Transform[] transforms;
    private UILineRenderer lineRenderer;
    private Canvas canvas;
    void OnEnable () {
        lineRenderer = GetComponent<UILineRenderer>();
        canvas = GetComponentInParent<Canvas>();
	}
	
	void Update () {
        var list = new Vector2[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
		{
            var scale = canvas.transform.localScale.x;
            var x = (transforms[i].position.x-transform.position.x ) / scale;
            var y = (transforms[i].position.y- transform.position.y) / scale;
		    list[i] = new Vector2(x,y);	 
		}

        lineRenderer.Points = list;
        lineRenderer.SetVerticesDirty();
	}
}
