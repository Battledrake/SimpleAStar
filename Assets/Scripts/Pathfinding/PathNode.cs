using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathNode : Node<PathNode>, IComparable<PathNode>
{
    public float _terrainCost = 0;

    public float _traversalCost = Mathf.Infinity;
    public float _totalCost = Mathf.Infinity;

    public PathNode _previous = null;

    public bool _isOpened = false;
    public bool _isClosed = false;


    public PathNode(GraphPosition graphPosition) : base(graphPosition) { }

    public int CompareTo(PathNode other)
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

    public override void Reset()
    {
        _traversalCost = Mathf.Infinity;
        _totalCost = Mathf.Infinity;
        _previous = null;
        _isOpened = false;
        _isClosed = false;
    }
}
