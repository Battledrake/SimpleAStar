using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public enum GraphConnections
    {
        Cardinal,
        Eight
    }

    [SerializeField] private GraphConnections _connections;


    public Node[,] Nodes => _nodes;
    public int Width => _width;
    public int Height => _height;

    private Node[,] _nodes;
    private int[,] _mapData;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private int _cellSize;


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

    public void Init()
    {
        _nodes = new Node[_width, _height];
        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                GraphPosition graphPosition = new GraphPosition(x, z);
                Node newNode = new Node(graphPosition, 0, false);
                _nodes[x, z] = newNode;

                newNode._position = new Vector3(x * _cellSize + _cellSize / 2, 0, z * _cellSize + _cellSize / 2);
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

    public void Init(int[,] mapData)
    {
        _mapData = mapData;
        _width = mapData.GetLength(0);
        _height = mapData.GetLength(1);
        _nodes = new Node[_width, _height];
        for (int z = 0; z < _height; z++)
        {
            for (int x = 0; x < _width; x++)
            {
                bool isBlocked = mapData[x, z] == 9;
                int terrainCost = mapData[x, z];
                GraphPosition graphPosition = new GraphPosition(x, z);
                Node newNode = new Node(graphPosition, terrainCost, isBlocked);
                _nodes[x, z] = newNode;

                newNode._position = new Vector3(x * _cellSize, 0, z * _cellSize);
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

    public GraphPosition GetGraphPositionFromWorld(Vector3 WorldPosition)
    {
        return new GraphPosition(
            Mathf.FloorToInt(WorldPosition.x / _cellSize),
            Mathf.FloorToInt(WorldPosition.z / _cellSize));
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
