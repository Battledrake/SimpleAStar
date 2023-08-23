using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node<T> where T : Node<T>
{
    public GraphPosition _graphPosition;

    public List<T> _neighbors = new List<T>();

    public Node(GraphPosition graphPosition)
    {
        _graphPosition = graphPosition;
    }

    public abstract void Reset();
}
