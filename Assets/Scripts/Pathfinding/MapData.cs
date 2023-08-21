using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

public enum GraphConnections
{
    Cardinal,
    Eight
}

public class MapData : MonoBehaviour
{
    private enum MapCreationType
    {
        InspectorValues,
        TextAsset,
        TextureMap
    }

    [SerializeField, HideInInspector, RuntimeReadOnly] private MapCreationType _mapCreationType;

    [SerializeField, HideInInspector, RuntimeReadOnly] private int _mapWidth = 10;
    [SerializeField, HideInInspector, RuntimeReadOnly] private int _mapHeight = 10;
    [SerializeField, HideInInspector, RuntimeReadOnly] private TextAsset _textMap;
    [SerializeField, HideInInspector, RuntimeReadOnly] private Texture2D _textureMap;

    [SerializeField, HideInInspector, RuntimeReadOnly] private Color32 _lightTerrainColor = new Color32(124, 194, 78, 255);
    [SerializeField, HideInInspector, RuntimeReadOnly] private Color32 _mediumTerrainColor = new Color32(252, 255, 52, 255);
    [SerializeField, HideInInspector, RuntimeReadOnly] private Color32 _heavyTerrainColor = new Color32(255, 129, 12, 255);

    [SerializeField, RuntimeReadOnly] private int _cellSize = 1;

    [SerializeField, RuntimeReadOnly] private Color32 _openColor = Color.grey;
    [SerializeField, RuntimeReadOnly] private Color32 _blockedColor = Color.black;

    [SerializeField, RuntimeReadOnly] private GraphConnections _connections;
    [SerializeField, RuntimeReadOnly] private GraphView _graphView;
    [SerializeField, RuntimeReadOnly] private bool _hideGraphViewOnPlay;

    public Graph GetGraph() => _graph;

    private static Dictionary<Color32, int> _terrainLookupTable = new Dictionary<Color32, int>();

    private int _graphWidth;
    private int _graphHeight;
    private Graph _graph;

    private void Awake()
    {
        SetupLookupTable();

        CreateGraph();
    }

    private void OnDrawGizmos()
    {
        if (_mapCreationType == MapCreationType.InspectorValues
            && Selection.activeGameObject == this.gameObject)
        {
            Gizmos.color = Color.cyan;
            Vector3 position = this.transform.position;
            Vector3 cubeCenter = new Vector3(
                position.x + _mapWidth * _cellSize * 0.5f,
                this.transform.position.y,
                position.z + _mapHeight * _cellSize * 0.5f);
            Gizmos.DrawWireCube(cubeCenter, new Vector3(_mapWidth * _cellSize, 0.5f, _mapHeight * _cellSize));
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

    public void SetGraphPositionBlocked(GraphPosition graphPosition)
    {
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graph.SetNodeBlockedState(graphPosition, true);
            _graphView.SetNodeViewColor(graphPosition, _blockedColor);
        }
    }

    public void SetGraphPositionUnblocked(GraphPosition graphPosition)
    {
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graph.SetNodeBlockedState(graphPosition, false);
            _graphView.SetNodeViewColor(graphPosition, _openColor);
        }
    }

    private void SetupLookupTable()
    {
        _terrainLookupTable.Add(_openColor, 0);
        _terrainLookupTable.Add(_blockedColor, 1);
        _terrainLookupTable.Add(_lightTerrainColor, 2);
        _terrainLookupTable.Add(_mediumTerrainColor, 3);
        _terrainLookupTable.Add(_heavyTerrainColor, 4);
    }

    public static Color GetColorFromTerrainCost(int terrainCost)
    {
        if (_terrainLookupTable.ContainsValue(terrainCost))
        {
            Color colorKey = _terrainLookupTable.FirstOrDefault(x => x.Value == terrainCost).Key;
            return colorKey;
        }
        return Color.white;
    }

    public List<string> GetMapFromTextFile(TextAsset textAsset)
    {
        List<string> lines = new List<string>();

        if (textAsset != null)
        {
            string textData = textAsset.text;
            string[] delimiters = { "\r\n", "\n" };
            lines.AddRange(textData.Split(delimiters, System.StringSplitOptions.None));
            lines.Reverse();
        }
        else
        {
            Debug.LogError("MAPDATA GetTextFromFile Error: Invalid TextAsset");
        }

        return lines;
    }

    public List<string> GetMapFromTexture(Texture2D texture)
    {
        List<string> lines = new List<string>();

        if (texture == null) return lines;

        for (int y = 0; y < texture.height; y++)
        {
            string newLine = "";

            for (int x = 0; x < texture.width; x++)
            {
                Color pixelColor = texture.GetPixel(x, y);
                if (_terrainLookupTable.ContainsKey(pixelColor))
                {
                    int terrainCost = _terrainLookupTable[pixelColor];
                    newLine += terrainCost;
                }
                else
                {
                    newLine += '0';
                }
            }
            lines.Add(newLine);
        }

        return lines;
    }

    public void SetDimensions(List<string> textLines)
    {
        _graphHeight = textLines.Count;
        foreach (string line in textLines)
        {
            if (line.Length > _mapWidth)
            {
                _graphWidth = line.Length;
            }
        }
    }

    public void CreateGraph()
    {
        List<string> lines = new List<string>();

        switch (_mapCreationType)
        {
            case MapCreationType.InspectorValues:
                _graphWidth = _mapWidth;
                _graphHeight = _mapHeight;
                break;
            case MapCreationType.TextAsset:
                lines = GetMapFromTextFile(_textMap);
                break;
            case MapCreationType.TextureMap:
                lines = GetMapFromTexture(_textureMap);
                break;
        }
        if (lines.Count > 0)
            SetDimensions(lines);

        _graph = new Graph(_connections, _graphWidth, _graphHeight, _cellSize);
        _graphView.Init(_graph, _cellSize);

        for (int z = 0; z < _graphHeight; z++)
        {
            for (int x = 0; x < _graphWidth; x++)
            {
                if (lines.Count > 0)
                {
                    int lineValue = (int)Char.GetNumericValue(lines[z][x]);

                    if (lineValue == 1)
                    {
                        _graph.SetNodeBlockedState(new GraphPosition(x, z), true);
                        _graphView.SetNodeViewColor(new GraphPosition(x, z), _blockedColor);
                    }
                }
                _graph.SetNodeBlockedState(new GraphPosition(x, z), false);
                _graphView.SetNodeViewColor(new GraphPosition(x, z), _openColor);
            }
        }

        if (_hideGraphViewOnPlay)
        {
            _graphView.HideGraphView();
        }
    }
}
