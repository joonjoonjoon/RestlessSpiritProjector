using UnityEngine;
using System.Collections;


[System.Serializable]
public class Vector2Int : System.IEquatable<Vector3Int>
{
    public int x, y;
    public Vector2Int(int x, int y) { this.x = x; this.y = y;}
    public Vector2Int() { }

    public bool Equals(Vector3Int other)
    {
        return x == other.x && y == other.y;
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

    public Vector2Int GetCopy()
    {
        return new Vector2Int(x, y);
    }

    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        return !(a == b);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x - b.x, a.y - b.y);
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new Vector2Int(a.x + b.x, a.y + b.y);
    }


}