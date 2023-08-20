using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class MapData : MonoBehaviour
{
    public int _width = 10;
    public int _height = 5;

    public TextAsset _textAsset;
    public Texture2D _textureMap;
    public string _resourcePath = "MapData";

    public Color32 _blockedColor = Color.black;
    public Color32 _openTerrainColor = Color.white;
    public Color32 _lightTerrainColor = new Color32(124, 194, 78, 255);
    public Color32 _mediumTerrainColor = new Color32(252, 255, 52, 255);
    public Color32 _heavyTerrainColor = new Color32(255, 129, 12, 255);

    private static Dictionary<Color32, int> _terrainLookupTable = new Dictionary<Color32, int>();

    private void Awake()
    {
        SetupLookupTable();

        string levelName = SceneManager.GetActiveScene().name;
        if (_textureMap == null && _textAsset == null)
        {
            _textureMap = Resources.Load<Texture2D>(_resourcePath + "/" + levelName);
        }

        if(_textAsset == null)
        {
            _textAsset = Resources.Load<TextAsset>(_resourcePath + "/" + levelName);
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
        _height = textLines.Count;
        foreach (string line in textLines)
        {
            if (line.Length > _width)
            {
                _width = line.Length;
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
            lines = GetMapFromTextFile(_textAsset);
        }

        SetDimensions(lines);

        int[,] map = new int[_width, _height];
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
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
