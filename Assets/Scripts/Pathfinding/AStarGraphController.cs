using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;
using Unity.VisualScripting;

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

public class AStarGraphController : MonoBehaviour
{
    private enum GraphCreationType
    {
        InspectorValues,
        TextureMap
    }

    [SerializeField, HideInInspector, RuntimeReadOnly] private GraphCreationType _graphCreationType;

    [SerializeField, HideInInspector, RuntimeReadOnly] private int _width = 10;
    [SerializeField, HideInInspector, RuntimeReadOnly] private int _height = 10;
    [SerializeField, HideInInspector, RuntimeReadOnly] private Texture2D _textureMap;

    [SerializeField, RuntimeReadOnly] private int _cellSize = 1;

    [SerializeField, RuntimeReadOnly] private Color32 _openColor = Color.grey;
    [SerializeField, RuntimeReadOnly] private Color32 _blockedColor = Color.black;
    [SerializeField] private List<TerrainData> _terrainData = new List<TerrainData>();

    [SerializeField, RuntimeReadOnly] private GraphConnectionType _connectionType;
    [SerializeField, RuntimeReadOnly] private GraphView _graphView;
    [SerializeField, RuntimeReadOnly] private bool _showGraphViewOnPlay;

    public Graph GetGraph() => _graph;

    private int _graphWidth;
    private int _graphHeight;
    private Graph _graph;

    private void Awake()
    {
        CreateGraph();
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

    public int GetPathLengthFromMoveLimit(List<GraphPosition> graphPositions, int moveLimit)
    {
        float totalTravel = 0;
        int pathLength = 0;
        for (int i = 0; i < graphPositions.Count; ++i)
        {
            float nodeCost = _graph.GetNodeTerrainCost(graphPositions[i]);
            if (nodeCost + totalTravel <= moveLimit)
            {
                totalTravel += nodeCost + 1;
                pathLength = i;
            }
            else
                break;
        }
        return pathLength;
    }

    public List<GraphPosition> ShrinkPathToMoveLimit(List<GraphPosition> graphPositions, int moveLimit)
    {
        List<GraphPosition> convertedList = new List<GraphPosition>();
        float totalTravel = 0;
        for (int i = 0; i < graphPositions.Count; ++i)
        {
            float nodeCost = _graph.GetNodeTerrainCost(graphPositions[i]);
            if (nodeCost + totalTravel <= moveLimit)
            {
                totalTravel += nodeCost + i + 1;
                convertedList.Add(graphPositions[i]);
            }
            else
                break;
        }
        return convertedList;
    }

    public void SetBlockedNodeFromWorldPosition(Vector3 worldPosition)
    {
        GraphPosition graphPosition = GetGraphPositionFromWorld(worldPosition);
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graph.SetNodeBlockedState(graphPosition, true);
            _graphView.SetNodeViewColor(graphPosition, _blockedColor);
        }
    }

    public void SetUnblockedNodeFromWorldPosition(Vector3 worldPosition)
    {
        GraphPosition graphPosition = GetGraphPositionFromWorld(worldPosition);
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graph.SetNodeBlockedState(graphPosition, false);
            _graphView.SetNodeViewColor(graphPosition, _openColor);
        }
    }

    public void SetBlockedNodeFromGraphPosition(GraphPosition graphPosition)
    {
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graph.SetNodeBlockedState(graphPosition, true);
            _graphView.SetNodeViewColor(graphPosition, _blockedColor);
        }
    }

    public void SetUnblockedNodeFromGraphPosition(GraphPosition graphPosition)
    {
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graph.SetNodeBlockedState(graphPosition, false);
            _graphView.SetNodeViewColor(graphPosition, _openColor);
        }
    }

    public bool IsValidTerrainColor(Color color)
    {
        return !_terrainData.FirstOrDefault(x => x._terrainColor == color).IsUnityNull();
    }

    public bool IsValidTerrainCost(int terrainCost)
    {
        return !_terrainData.FirstOrDefault(x => x._terrainCost == terrainCost).IsUnityNull();
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

    public void SetDimensions(List<string> textLines)
    {
        _graphHeight = textLines.Count;
        foreach (string line in textLines)
        {
            if (line.Length > _width)
            {
                _graphWidth = line.Length;
            }
        }
    }

    public void CreateGraph()
    {
        List<string> lines = new List<string>();

        switch (_graphCreationType)
        {
            case GraphCreationType.InspectorValues:
                _graphWidth = _width;
                _graphHeight = _height;
                break;
            case GraphCreationType.TextureMap:
                _graphWidth = _textureMap.width;
                _graphHeight = _textureMap.height;
                break;
        }
        if (lines.Count > 0)
            SetDimensions(lines);

        _graph = new Graph(_connectionType, _graphWidth, _graphHeight);
        _graphView.Init(_graph, _cellSize);

        for (int z = 0; z < _graphHeight; z++)
        {
            for (int x = 0; x < _graphWidth; x++)
            {
                GraphPosition nodePosition = new GraphPosition(x, z);
                if (_graphCreationType == GraphCreationType.TextureMap)
                {
                    Color nodeColor = _textureMap.GetPixel(x, z);
                    float terrainCost = GetTerrainCostFromColor(nodeColor);
                    if (terrainCost == 0)
                        nodeColor = _openColor;
                    bool nodeBlocked = nodeColor == _blockedColor ? true : false;
                    _graph.SetNodeBlockedState(nodePosition, nodeBlocked);
                    _graph.SetNodeTerrainCost(nodePosition, terrainCost);
                    _graphView.SetNodeViewColor(nodePosition, nodeColor);
                }
                else
                {
                    _graph.SetNodeBlockedState(nodePosition, false);
                    _graphView.SetNodeViewColor(nodePosition, _openColor);
                }
            }
        }

        if (!_showGraphViewOnPlay)
        {
            _graphView.HideGraphView();
        }
    }
}
