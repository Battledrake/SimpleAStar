using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public GraphPosition _graphPosition;

    public int _terrainCost;
    public Vector3 _position;

    public List<Node> _neighbors = new List<Node>();

    public float _distanceTravelled = Mathf.Infinity;

    public Node _previous = null;

    public float _priority = Mathf.Infinity;

    public bool _isOpened = false;
    public bool _isClosed = false;

    public bool _isBlocked = false;

    public Node(GraphPosition graphPosition, int terrainCost, bool isBlocked)
    {
        _graphPosition = graphPosition;
        _terrainCost = terrainCost;
        _isBlocked = isBlocked;
    }

    public int CompareTo(Node other)
    {
        if (this._priority < other._priority)
        {
            return -1;
        }
        else if (this._priority > other._priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void Reset()
    {
        _distanceTravelled = Mathf.Infinity;
        _previous = null;
        _priority = Mathf.Infinity;
        _isOpened = false;
        _isClosed = false;
    }
}
