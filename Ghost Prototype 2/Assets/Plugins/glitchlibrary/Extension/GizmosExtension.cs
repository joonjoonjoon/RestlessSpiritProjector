using UnityEngine;
using System.Collections;

public class GizmosExtension : MonoBehaviour {

    public static void DrawRect(Rect rect, Transform relativeTo= null)
    {
        if (relativeTo == null)
        {
            Gizmos.DrawLine(
                new Vector3(rect.x,rect.y,0),
                new Vector3(rect.x + rect.width, rect.y, 0)
            );
            Gizmos.DrawLine(
                new Vector3(rect.x, rect.y, 0),
                new Vector3(rect.x, rect.y+rect.height, 0)
            );
            Gizmos.DrawLine(
                new Vector3(rect.x+rect.width, rect.y + rect.height, 0),
                new Vector3(rect.x, rect.y + rect.height, 0)
            );
            Gizmos.DrawLine(
                new Vector3(rect.x + rect.width, rect.y + rect.height, 0),
                new Vector3(rect.x + rect.width, rect.y, 0)
            );
        }
        else
        {
            Gizmos.DrawLine(
                relativeTo.position + relativeTo.TransformVector(new Vector3(-rect.width / 2, -rect.height / 2, 0)),
                relativeTo.position + relativeTo.TransformVector(new Vector3(rect.width / 2, -rect.height / 2, 0))
            );
            Gizmos.DrawLine(
                relativeTo.position + relativeTo.TransformVector(new Vector3(-rect.width / 2, -rect.height / 2, 0)),
                relativeTo.position + relativeTo.TransformVector(new Vector3(-rect.width / 2, rect.height/2,0))
            );
            Gizmos.DrawLine(
                relativeTo.position + relativeTo.TransformVector(new Vector3(rect.width / 2, -rect.height/ 2, 0)),
                relativeTo.position + relativeTo.TransformVector(new Vector3(rect.width / 2, rect.height/2,0))
            );
            Gizmos.DrawLine(
                relativeTo.position + relativeTo.TransformVector(new Vector3(-rect.width / 2, rect.height/2,0)),
                relativeTo.position + relativeTo.TransformVector(new Vector3(rect.width / 2, rect.height/2,0))
            );
        }
    }

    public static void DrawPoly(params Vector3[] points)
    {
        for (int i = 0; i < points.Length-1; i++)
        {
            Gizmos.DrawLine(points[i], points[i+1]);
        }
        Gizmos.DrawLine(points[points.Length-1], points[0]);

    }

}
