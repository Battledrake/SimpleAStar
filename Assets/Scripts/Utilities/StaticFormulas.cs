using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticFormulas
{
    public static float Chebyshev(PathNode source, PathNode target)
    {
        GraphPosition distance = (source._graphPosition - target._graphPosition).Abs();
        return Mathf.Max(distance.x, distance.z);
    }

    public static float Diagonal(PathNode source, PathNode target)
    {
        GraphPosition distance = (source._graphPosition - target._graphPosition).Abs();
        float diagonal = Mathf.Sqrt(2);

        return (distance.x + distance.z) + (diagonal - 2) * Mathf.Min(distance.x, distance.z);
    }

    public static float DiagonalShortcut(PathNode source, PathNode target)
    {
        GraphPosition distance = (source._graphPosition - target._graphPosition).Abs();

        return (distance.x + distance.z) + (1.4f - 2) * Mathf.Min(distance.x, distance.z);
    }
    public static float Manhattan(PathNode source, PathNode target)
    {
        GraphPosition distance = (source._graphPosition - target._graphPosition).Abs();

        return distance.x + distance.z;
    }

    public static float Euclidean(PathNode source, PathNode target)
    {
        GraphPosition distance = (source._graphPosition - target._graphPosition).Abs();

        return Mathf.Sqrt(distance.x * distance.x + distance.z * distance.z);
    }
}
