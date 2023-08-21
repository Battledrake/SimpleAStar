using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    [SerializeField, HideInInspector] private MapCreationType _mapCreationType;

    [SerializeField, HideInInspector] private int _mapWidth = 10;
    [SerializeField, HideInInspector] private int _mapHeight = 10;
    [SerializeField, HideInInspector] private TextAsset _textMap;
    [SerializeField, HideInInspector] private Texture2D _textureMap;

    [SerializeField, HideInInspector] private Color32 _lightTerrainColor = new Color32(124, 194, 78, 255);
    [SerializeField, HideInInspector] private Color32 _mediumTerrainColor = new Color32(252, 255, 52, 255);
    [SerializeField, HideInInspector] private Color32 _heavyTerrainColor = new Color32(255, 129, 12, 255);

    [SerializeField] private Color32 _openColor = Color.grey;
    [SerializeField] private Color32 _blockedColor = Color.black;

    [SerializeField] private int _cellSize = 1;
    [SerializeField] private GraphConnections _connections;
    [SerializeField] private GraphView _graphView;

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

    public GraphPosition GetGraphPositionFromWorld(Vector3 worldPosition)
    {
        return new GraphPosition(
            Mathf.RoundToInt(worldPosition.x / _cellSize),
            Mathf.RoundToInt(worldPosition.z / _cellSize));
    }

    public Vector3 GetWorldPositionFromGraphPosition(GraphPosition graphPosition)
    {
        return new Vector3(graphPosition.x * _cellSize, 0f, graphPosition.z * _cellSize);
    }

    public List<Vector3> GetWorldPositionsFromGraphPositions(List<GraphPosition> graphPositions)
    {
        List<Vector3> vectorList = new List<Vector3>();
        foreach (GraphPosition position in graphPositions)
        {
            vectorList.Add(new Vector3(position.x * _cellSize, 0f, position.z * _cellSize));
        }
        return vectorList;
    }

    public void SetGraphPositionBlocked(GraphPosition graphPosition)
    {
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graphView.SetNodeViewColor(graphPosition, _blockedColor);
            _graph.SetNodeBlockedState(graphPosition, true);
        }
    }

    public void SetGraphPositionUnblocked(GraphPosition graphPosition)
    {
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graphView.SetNodeViewColor(graphPosition, _openColor);
            _graph.SetNodeBlockedState(graphPosition, false);
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

        //FIX: Realize that can't change connections at runtime, might want to change that.
        //Also create graph with blocked nodes needs to be re-implemented.
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
    }
}
