using UnityEngine;
using System.Collections;


public static class QuaternionExtension
{
    public static string Serialize(this Quaternion q)
    {
        return QuaternionToString(q);
    }

    public static string QuaternionToString(Quaternion quaternion)
    {
        return quaternion.x + ";" + quaternion.y + ";" + quaternion.z + ";" + quaternion.w;
    }

    public static Quaternion StringToQuaternion(string vector)
    {
        var coords = vector.Split(';');
        return new Quaternion(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]), float.Parse(coords[3]));
    }

    public static Quaternion withX(this Quaternion parent, float x)
    {
        return new Quaternion(x, parent.y, parent.z, parent.w);
    }

    public static Quaternion withY(this Quaternion parent, float y)
    {
        return new Quaternion(parent.x, y, parent.z, parent.w);
    }

    public static Quaternion withZ(this Quaternion parent, float z)
    {
        return new Quaternion(parent.x, parent.y, z, parent.w);
    }

    public static Quaternion withW(this Quaternion parent, float w)
    {
        return new Quaternion(parent.x, parent.y, parent.z, w);
    }

}
