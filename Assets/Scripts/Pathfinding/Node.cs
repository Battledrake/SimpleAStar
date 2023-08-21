using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public GraphPosition _graphPosition;

    public int _terrainCost = 0;

    public float _traversalCost = Mathf.Infinity;
    public float _totalCost = Mathf.Infinity;

    public List<Node> _neighbors = new List<Node>();

    public Node _previous = null;

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
        if (this._totalCost < other._totalCost)
        {
            return -1;
        }
        else if (this._totalCost > other._totalCost)
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
        _traversalCost = Mathf.Infinity;
        _totalCost = Mathf.Infinity;
        _previous = null;
        _isOpened = false;
        _isClosed = false;
    }
}
