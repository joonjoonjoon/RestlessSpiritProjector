using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Vector2Extension {
    public static Vector3 ToVector3(this Vector2 parent)
    {
        return new Vector3(parent.x, parent.y, 0);
    }

    public static Vector2 withX(this Vector2 parent, float x)
    {
        return new Vector2(x, parent.y);
    }

    public static Vector2 withXY(this Vector2 parent, float x, float y)
    {
        return new Vector2(x, y);
    }

    public static Vector2 withY(this Vector2 parent, float y)
    {
        return new Vector2(parent.x, y);
    }

}
