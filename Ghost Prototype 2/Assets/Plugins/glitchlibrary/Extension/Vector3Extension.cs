using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Vector3Extension
{
    public static Vector3 withX(this Vector3 parent, float x)
    {
        return new Vector3(x, parent.y, parent.z);
    }

    public static Vector3 withXY(this Vector3 parent, float x, float y)
    {
        return new Vector3(x, y, parent.z);
    }

    public static Vector3 withY(this Vector3 parent, float y)
    {
        return new Vector3(parent.x, y, parent.z);
    }

    public static Vector3 withZ(this Vector3 parent, float z)
    {
        return new Vector3(parent.x, parent.y, z);
    }

    public static Vector3 addX(this Vector3 parent, float x)
    {
        return new Vector3(parent.x + x, parent.y, parent.z);
    }

    public static Vector3 addY(this Vector3 parent, float y)
    {
        return new Vector3(parent.x, parent.y + y, parent.z);
    }

    public static Vector3 addZ(this Vector3 parent, float z)
    {
        return new Vector3(parent.x, parent.y, parent.z + z);
    }

    public static Vector3 addVector2(this Vector3 parent, Vector2 v2)
    {
        return new Vector3(parent.x + v2.x, parent.y + v2.y, parent.z);
    }

    public static float Distance2D(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v1.x - v2.x, 2) + Mathf.Pow(v1.y - v2.y, 2));

    }

    public static float Angle2D(Vector3 v1, Vector3 v2)
    {
        float result = (Mathf.Atan2(v2.y - v1.y, v2.x - v1.x) * Mathf.Rad2Deg);
        if (result < 0) result = result + 360;
        if (result > 360) result = result - 360;
        return result;
    }

    public static Vector3 Clamp(this Vector3 value, Vector3 min, Vector3 max)
    {
        value.x = Mathf.Clamp(value.x, min.x, max.x);
        value.y = Mathf.Clamp(value.y, min.y, max.y);
        value.z = Mathf.Clamp(value.z, min.z, max.z);

        return value;
    }

    public static Vector2 ToVector2IgnoreZ(this Vector3 parent)
    {
        return new Vector2(parent.x, parent.y);
    }


}
