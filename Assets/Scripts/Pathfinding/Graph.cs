using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph<T> where T : Node<T>
{
    private GraphConnectionType _connections;
    public T[,] Nodes => _nodes;
    public int Width => _width;
    public int Height => _height;

    private T[,] _nodes;
    private int _width;
    private int _height;

    public static readonly Vector2Int[] CardinalDirections =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    public static readonly Vector2Int[] EightDirections =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1)
    };

    public Graph(GraphConnectionType connections, int width, int height)
    {
        _connections = connections;
        _width = width;
        _height = height;

        _nodes = new T[_width, _height];
        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                GraphPosition graphPosition = new GraphPosition(x, z);
                T newNode = (T)Activator.CreateInstance(typeof(T), graphPosition);
                _nodes[x, z] = newNode;
            }
        }

        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                _nodes[x, z]._neighbors = GetNeighbors(new GraphPosition(x, z));
            }
        }
    }

    public void ResetNodes()
    {
        for (int x = 0; x < _width; ++x)
        {
            for (int y = 0; y < _height; ++y)
            {
                _nodes[x, y].Reset();
            }
        }
    }

    public T GetNodeFromGraphPosition(GraphPosition graphPosition)
    {
        if (IsWithinBounds(graphPosition))
        {
            return _nodes[graphPosition.x, graphPosition.z];
        }
        return null;
    }

    public bool IsWithinBounds(GraphPosition graphPosition)
    {
        return (graphPosition.x >= 0 && graphPosition.x < _width && graphPosition.z >= 0 && graphPosition.z < _height);
    }

    public List<T> GetNeighbors(GraphPosition graphPosition)
    {
        Vector2Int[] directions = GetDirectionsByType();
        List<T> neighborNodes = new List<T>();

        foreach (Vector2Int direction in directions)
        {
            GraphPosition neighborPosition = new GraphPosition(graphPosition.x + direction.x, graphPosition.z + direction.y);
            if (IsWithinBounds(neighborPosition))
            {
                neighborNodes.Add(_nodes[neighborPosition.x, neighborPosition.z]);
            }
        }

        return neighborNodes;
    }

    public Vector2Int[] GetDirectionsByType()
    {
        switch (_connections)
        {
            case GraphConnectionType.Cardinal:
                return CardinalDirections;
            case GraphConnectionType.Eight:
                return EightDirections;
            default:
                return new Vector2Int[0];
        }
    }
}
