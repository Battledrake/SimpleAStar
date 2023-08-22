using System;
using UnityEngine;

public struct GraphPosition : IEquatable<GraphPosition>
{
    public int x;
    public int z;

    public GraphPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public GraphPosition Abs()
    {
        return new GraphPosition(Mathf.Abs(x), Mathf.Abs(z));
    }

    public static bool operator ==(GraphPosition a, GraphPosition b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool operator !=(GraphPosition a, GraphPosition b)
    {
        return !(a == b);
    }

    public static GraphPosition operator +(GraphPosition a, GraphPosition b)
    {
        return new GraphPosition(a.x + b.x, a.z + b.z);
    }

    public static GraphPosition operator +(GraphPosition a, Vector2Int b)
    {
        return new GraphPosition(a.x + b.x, a.z + b.y);
    }

    public static GraphPosition operator -(GraphPosition a, GraphPosition b)
    {
        return new GraphPosition(a.x - b.x, a.z - b.z);
    }

    public static GraphPosition operator -(GraphPosition a, Vector2Int b)
    {
        return new GraphPosition(a.x - b.x, a.z - b.y);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public override bool Equals(object obj)
    {
        return obj is GraphPosition graphPosition &&
            x == graphPosition.x &&
            z == graphPosition.z;
    }

    public bool Equals(GraphPosition other)
    {
        return this == other;
    }

    public override string ToString()
    {
        return $"{x}, {z}";
    }
}