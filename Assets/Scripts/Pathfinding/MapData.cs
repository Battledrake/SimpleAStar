using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public enum GraphConnections
{
    Cardinal,
    Eight
}

public class MapData : MonoBehaviour
{
    [SerializeField] private int _mapWidth = 10;
    [SerializeField] private int _mapHeight = 10;
    [SerializeField] private int _cellSize = 1;
    [SerializeField] private GraphConnections _connections;
    [SerializeField] private GraphView _graphView;
    [SerializeField] private Pathfinder _pathfinder;

    [SerializeField] private TextAsset _textMap;
    [SerializeField] private Texture2D _textureMap;
    [Tooltip("Path to auto load data if not inserted")]
    [SerializeField] private string _resourcePath = "MapData";

    [SerializeField] private Color32 _blockedColor = Color.black;
    [SerializeField] private Color32 _openTerrainColor = Color.white;
    [SerializeField] private Color32 _lightTerrainColor = new Color32(124, 194, 78, 255);
    [SerializeField] private Color32 _mediumTerrainColor = new Color32(252, 255, 52, 255);
    [SerializeField] private Color32 _heavyTerrainColor = new Color32(255, 129, 12, 255);

    public Graph GetGraph() => _graph;

    private static Dictionary<Color32, int> _terrainLookupTable = new Dictionary<Color32, int>();

    private Graph _graph;

    private void Awake()
    {
        SetupLookupTable();

        _graph = new Graph(_connections, _mapWidth, _mapHeight, _cellSize);
        _graphView.Init(_graph);

        //string levelName = SceneManager.GetActiveScene().name;
        //if (_textureMap == null && _textMap == null)
        //{
        //    _textureMap = Resources.Load<Texture2D>(_resourcePath + "/" + levelName);
        //}

        //if(_textMap == null)
        //{
        //    _textMap = Resources.Load<TextAsset>(_resourcePath + "/" + levelName);
        //}
    }

    public PathResult FindPath(GraphPosition startPosition, GraphPosition endPosition, out List<GraphPosition> pathPositions)
    {
        pathPositions = new List<GraphPosition>();
        PathResult checkResult = _pathfinder.FindPath(startPosition, endPosition, _graph, ref pathPositions);
        return checkResult;
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
        foreach(GraphPosition position in graphPositions)
        {
            vectorList.Add(new Vector3(position.x * _cellSize, 0f, position.z * _cellSize));
        }
        return vectorList;
    }

    public void SetGraphPositionBlocked(GraphPosition graphPosition)
    {
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graphView.SetViewColorFromIsBlocked(graphPosition, true);
            _graph.SetNodeIsBlocked(graphPosition, true);
        }
    }

    public void SetGraphPositionUnblocked(GraphPosition graphPosition)
    {
        if (_graph.IsWithinBounds(graphPosition))
        {
            _graphView.SetViewColorFromIsBlocked(graphPosition, false);
            _graph.SetNodeIsBlocked(graphPosition, false);
        }
    }

    private void SetupLookupTable()
    {
        _terrainLookupTable.Add(_openTerrainColor, 0);
        _terrainLookupTable.Add(_lightTerrainColor, 1);
        _terrainLookupTable.Add(_mediumTerrainColor, 2);
        _terrainLookupTable.Add(_heavyTerrainColor, 3);
        _terrainLookupTable.Add(_blockedColor, 9);
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
        _mapHeight = textLines.Count;
        foreach (string line in textLines)
        {
            if (line.Length > _mapWidth)
            {
                _mapWidth = line.Length;
            }
        }
    }

    public int[,] MakeMap()
    {
        List<string> lines = new List<string>();

        if (_textureMap != null)
        {
            lines = GetMapFromTexture(_textureMap);
        }
        else
        {
            lines = GetMapFromTextFile(_textMap);
        }

        SetDimensions(lines);

        int[,] map = new int[_mapWidth, _mapHeight];
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                if (lines[y].Length > x)
                {
                    map[x, y] = (int)Char.GetNumericValue(lines[y][x]);
                }
            }
        }
        return map;
    }
}
