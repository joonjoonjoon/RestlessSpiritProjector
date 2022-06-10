using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIPolygonRenderer), typeof(PolygonCollider2D))]
[ExecuteInEditMode]
public class UIPolygonRendererTransformController : MonoBehaviour
{
    public Transform[] transforms;
    private PolygonCollider2D polygonCollider;
    private Canvas canvas;

    void OnEnable()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (transforms == null || transforms.Length == 0 || transforms[0] == null)
            return;

        var list = new Vector2[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            var scale = canvas.transform.localScale.x;
            var x = (transforms[i].position.x - transform.position.x) / scale;
            var y = (transforms[i].position.y - transform.position.y) / scale;
            list[i] = new Vector2(x, y); 
        }

        polygonCollider.points = list;
        GetComponent<UIPolygonRenderer>().SetVerticesDirty();
    }
}
