using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public enum DirectionType
    {
        Cardinal,
        Hex,
        All
    }

    [SerializeField] private DirectionType _directionType;


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

    public static readonly Vector2Int[] HexDirections =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1)
    };

    public static readonly Vector2Int[] AllDirections =
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

                newNode._position = new Vector3(x, 0, z);
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

    public List<Node> GetNeighbors(GraphPosition graphPosition)
    {
        Vector2Int[] directions = GetDirectionsByType(_directionType);
        List<Node> neighborNodes = new List<Node>();

        foreach (Vector2Int dir in directions)
        {
            //int newX = x + dir.x;
            //int newY = y + dir.y;
            int newX = graphPosition.x + dir.x;
            int newZ = graphPosition.z + dir.y;

            if (IsWithinBounds(graphPosition + dir)
                && (!_nodes[newX, newZ]._isBlocked))
            {
                neighborNodes.Add(_nodes[newX, newZ]);
            }
        }

        return neighborNodes;
    }

    public Vector2Int[] GetDirectionsByType(DirectionType neighborType)
    {
        switch (neighborType)
        {
            case DirectionType.Cardinal:
                return CardinalDirections;
            case DirectionType.Hex:
                return HexDirections;
            case DirectionType.All:
                return AllDirections;
            default:
                return new Vector2Int[0];
        }
    }
}
