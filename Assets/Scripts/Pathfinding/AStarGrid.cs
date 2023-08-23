using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

public enum GraphConnectionType
{
    Cardinal,
    Eight
}

[Serializable]
public struct TerrainData
{
    public Color32 _terrainColor;
    public float _terrainCost;
}

public class AStarGrid : MonoBehaviour
{
    private enum GraphCreationType
    {
        InspectorValues,
        TextureMap,
        Custom
    }

    [SerializeField, HideInInspector, RuntimeReadOnly] private GraphCreationType _graphCreationType;

    [SerializeField, HideInInspector, RuntimeReadOnly] private int _width = 10;
    [SerializeField, HideInInspector, RuntimeReadOnly] private int _height = 10;
    [SerializeField, HideInInspector, RuntimeReadOnly] private Texture2D _textureMap;

    [SerializeField, HideInInspector, RuntimeReadOnly] private int _cellSize = 1;
    [SerializeField, HideInInspector, RuntimeReadOnly] private GraphConnectionType _connectionType;

    [SerializeField, RuntimeReadOnly] private Color32 _openColor = Color.grey;
    [SerializeField, RuntimeReadOnly] private Color32 _blockedColor = Color.black;
    [SerializeField] private List<TerrainData> _terrainData = new List<TerrainData>();

    [SerializeField, RuntimeReadOnly] private GraphView _graphView;
    [SerializeField, RuntimeReadOnly] private bool _showGraphViewOnCreate;

    public Graph<PathNode> GetGraph() => _graph;

    private int _graphWidth;
    private int _graphHeight;
    private Graph<PathNode> _graph;

    private void Awake()
    {
        switch (_graphCreationType)
        {
            case GraphCreationType.InspectorValues:
                Init(_width, _height, _cellSize, _connectionType);
                break;
            case GraphCreationType.TextureMap:
                Init(_textureMap.width, _textureMap.height, _cellSize, _connectionType);
                break;
        }
    }

    public void Init(int width, int height, int cellSize, GraphConnectionType connectionType)
    {
        _graphWidth = width;
        _graphHeight = height;
        _cellSize = cellSize;
        _connectionType = connectionType;

        CreateGraph();
    }

    public void CreateGraph()
    {
        _graph = new Graph<PathNode>(_connectionType, _graphWidth, _graphHeight);
        _graphView.Init(_graph, _cellSize);

        for (int z = 0; z < _graphHeight; z++)
        {
            for (int x = 0; x < _graphWidth; x++)
            {
                GraphPosition nodePosition = new GraphPosition(x, z);
                PathNode pathNode = _graph.GetNodeFromGraphPosition(nodePosition);
                if (_graphCreationType == GraphCreationType.TextureMap)
                {
                    Color nodeColor = _textureMap.GetPixel(x, z);
                    float terrainCost = GetTerrainCostFromColor(nodeColor);
                    if (terrainCost == 0)
                        nodeColor = _openColor;
                    bool nodeBlocked = nodeColor == _blockedColor ? true : false;
                    pathNode._isBlocked = nodeBlocked;
                    pathNode._terrainCost = terrainCost;
                    _graphView.SetNodeViewColor(nodePosition, nodeColor);
                }
                else
                {
                    pathNode._isBlocked = false;
                    _graphView.SetNodeViewColor(nodePosition, _openColor);
                }
            }
        }

        if (!_showGraphViewOnCreate)
        {
            _graphView.HideGraphView();
        }
    }

    private void OnDrawGizmos()
    {
        if (_graphCreationType == GraphCreationType.InspectorValues
            && Selection.activeGameObject == this.gameObject)
        {
            Gizmos.color = Color.cyan;
            Vector3 position = this.transform.position;
            Vector3 cubeCenter = new Vector3(
                position.x + _width * _cellSize * 0.5f,
                this.transform.position.y,
                position.z + _height * _cellSize * 0.5f);
            Gizmos.DrawWireCube(cubeCenter, new Vector3(_width * _cellSize, 0.5f, _height * _cellSize));
        }
    }

    public GraphPosition GetGraphPositionFromWorld(Vector3 worldPosition)
    {
        return new GraphPosition(
            Mathf.FloorToInt((worldPosition.x - this.transform.position.x) / _cellSize),
            Mathf.FloorToInt((worldPosition.z - this.transform.position.z) / _cellSize));
    }

    public Vector3 GetWorldPositionFromGraphPosition(GraphPosition graphPosition)
    {
        return new Vector3(
            this.transform.position.x + graphPosition.x * _cellSize + _cellSize * 0.5f,
            this.transform.position.y,
            this.transform.position.z + graphPosition.z * _cellSize + _cellSize * 0.5f);
    }

    public List<Vector3> GetWorldPositionsFromGraphPositions(List<GraphPosition> graphPositions)
    {
        List<Vector3> vectorList = new List<Vector3>();
        foreach (GraphPosition position in graphPositions)
        {
            vectorList.Add(new Vector3(
                this.transform.position.x + position.x * _cellSize + _cellSize * 0.5f,
                this.transform.position.y,
                this.transform.position.z + position.z * _cellSize + _cellSize * 0.5f));
        }
        return vectorList;
    }

    public int GetPathLengthFromMoveLimit(List<Vector3> positions, int moveLimit)
    {
        float totalTravel = 0;
        int pathLength = 0;
        for (int i = 0; i < positions.Count; ++i)
        {
            GraphPosition graphPosition = GetGraphPositionFromWorld(positions[i]);
            PathNode pathNode = _graph.GetNodeFromGraphPosition(graphPosition);
            float terrainCost = pathNode._terrainCost;
            if (terrainCost + totalTravel <= moveLimit)
            {
                totalTravel += terrainCost + 1;
                pathLength = i;
            }
            else
                break;
        }
        return pathLength;
    }

    public void SetNodeIsBlocked(Vector3 worldPosition, bool isBlocked)
    {
        GraphPosition graphPosition = GetGraphPositionFromWorld(worldPosition);
        PathNode pathNode = _graph.GetNodeFromGraphPosition(graphPosition);
        if (pathNode != null)
        {
            pathNode._isBlocked = isBlocked;
            Color nodeViewColor = isBlocked ? _blockedColor : _openColor;
            _graphView.SetNodeViewColor(graphPosition, nodeViewColor);
            UpdateNodeNeighbors(pathNode);
        }
    }

    public void UpdateNodeNeighbors(PathNode changedNode)
    {
        changedNode._neighbors.Clear();

        Vector2Int[] directions = _graph.GetDirectionsByType();
        
        for(int i = 0; i < directions.Length; i++)
        {
            GraphPosition neighborPosition = new GraphPosition(changedNode._graphPosition.x + directions[i].x, changedNode._graphPosition.z + directions[i].y);
            PathNode neighborNode = _graph.GetNodeFromGraphPosition(neighborPosition);

            if(neighborNode != null && !neighborNode._isBlocked)
            {
                List<PathNode> neighborNodes = new List<PathNode>();
                for(int j = 0; j < directions.Length; j++)
                {
                    GraphPosition neighborsNeighbor = new GraphPosition(neighborNode._graphPosition.x + directions[j].x, neighborNode._graphPosition.z + directions[j].y);
                    PathNode neighborsNeighborNode = _graph.GetNodeFromGraphPosition(neighborsNeighbor);

                    if(neighborsNeighborNode != null && !neighborsNeighborNode._isBlocked)
                    {
                        neighborNodes.Add(neighborsNeighborNode);
                    }
                }
                neighborNode._neighbors = neighborNodes;

                if (!changedNode._isBlocked)
                    changedNode._neighbors.Add(neighborNode);
            }
        }
    }

    public bool IsValidTerrainColor(Color color)
    {
        return !_terrainData.FirstOrDefault(x => x._terrainColor == color).Equals(null);
    }

    public bool IsValidTerrainCost(int terrainCost)
    {
        return !_terrainData.FirstOrDefault(x => x._terrainCost == terrainCost).Equals(null);
    }

    public float GetTerrainCostFromColor(Color color)
    {
        if (IsValidTerrainColor(color))
        {
            return _terrainData.FirstOrDefault(x => x._terrainColor == color)._terrainCost;
        }
        return 0;
    }

    public Color GetColorFromTerrainCost(int terrainCost)
    {
        if (IsValidTerrainCost(terrainCost))
        {
            return _terrainData.FirstOrDefault(x => x._terrainCost == terrainCost)._terrainColor;
        }
        return _openColor;
    }
}
