using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    private GraphConnections _connections;
    public Node[,] Nodes => _nodes;
    public int Width => _width;
    public int Height => _height;

    private Node[,] _nodes;
    private int[,] _mapData;
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

    public Graph(GraphConnections connections, int width, int height, int cellSize)
    {
        _connections = connections;
        _width = width;
        _height = height;

        _nodes = new Node[_width, _height];
        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                GraphPosition graphPosition = new GraphPosition(x, z);
                Node newNode = new Node(graphPosition, 0, false);
                _nodes[x, z] = newNode;
            }
        }

        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (!_nodes[x, z]._isBlocked)
                    _nodes[x, z]._neighbors = GetNeighbors(new GraphPosition(x, z));
            }
        }
    }

    public void ResetNodes()
    {
        for(int x = 0; x < _width; ++x)
        {
            for(int y = 0; y < _height; ++y)
            {
                _nodes[x, y].Reset();
            }
        }
    }

    public Node GetNodeFromGraphPosition(GraphPosition graphPosition)
    {
        if (IsWithinBounds(graphPosition))
        {
            return _nodes[graphPosition.x, graphPosition.z];
        }
        return null;
    }

    public void SetNodeIsBlocked(GraphPosition graphPosition, bool isBlocked)
    {
        if (IsWithinBounds(graphPosition))
        {
            Node node = GetNodeFromGraphPosition(graphPosition);
            node._isBlocked = isBlocked;

            for (int i = 0; i < node._neighbors.Count; i++)
            {
                node._neighbors[i]._neighbors = GetNeighbors(node._neighbors[i]._graphPosition);
            }

            if (node._isBlocked)
                node._neighbors.Clear();
            else
                node._neighbors = GetNeighbors(node._graphPosition);
        }
    }

    public bool IsWithinBounds(GraphPosition graphPosition)
    {
        return (graphPosition.x >= 0 && graphPosition.x < _width && graphPosition.z >= 0 && graphPosition.z < _height);
    }

    public bool IsGraphPositionTraversable(GraphPosition graphPosition)
    {
        if (IsWithinBounds(graphPosition))
        {
            return !_nodes[graphPosition.x, graphPosition.z]._isBlocked;
        }
        return false;
    }

    public List<Node> GetNeighbors(GraphPosition graphPosition)
    {
        Vector2Int[] directions = GetDirectionsByType(_connections);
        List<Node> neighborNodes = new List<Node>();

        foreach (Vector2Int direction in directions)
        {
            int newX = graphPosition.x + direction.x;
            int newZ = graphPosition.z + direction.y;

            if (IsWithinBounds(graphPosition + direction)
                && (!_nodes[newX, newZ]._isBlocked))
            {
                neighborNodes.Add(_nodes[newX, newZ]);
            }
        }

        return neighborNodes;
    }

    public Vector2Int[] GetDirectionsByType(GraphConnections neighborType)
    {
        switch (neighborType)
        {
            case GraphConnections.Cardinal:
                return CardinalDirections;
            case GraphConnections.Eight:
                return EightDirections;
            default:
                return new Vector2Int[0];
        }
    }
}
