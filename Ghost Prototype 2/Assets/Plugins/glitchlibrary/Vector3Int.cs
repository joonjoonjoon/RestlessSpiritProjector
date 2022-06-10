using UnityEngine;
using System.Collections;


[System.Serializable]
public class Vector3Int : System.IEquatable<Vector3Int>
{
    public int x, y, z;
    public Vector3Int(int x, int y, int z) { this.x = x; this.y = y; this.z = z; }
    public Vector3Int() { }

    public bool Equals(Vector3Int other)
    {
        return x == other.x && y == other.y && z == other.z;
    }
    public override bool Equals(object other) // this dumb method exists to prevent a dumb warning.
    {
        Debug.Log("Not implemented");
        return false;
    }

    public override int GetHashCode()   // this dumb method exists to prevent a dumb warning.
    {
        return base.GetHashCode();
    }

    public Vector3Int GetCopy()
    {
        return new Vector3Int(x, y, z);
    }

    public static bool operator ==(Vector3Int a, Vector3Int b)
    {
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(Vector3Int a, Vector3Int b)
    {
        return !(a == b);
    }

    public static Vector3Int operator -(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vector3Int operator +(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
    }


}