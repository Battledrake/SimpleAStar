using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using Unity.VisualScripting;

public class NodeView : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _debugText;

    [SerializeField] private GameObject _tile;
    [SerializeField] private Renderer _tileRenderer;

    private GraphPosition _graphPosition;

    public void Init(GraphPosition graphPosition, int cellSize)
    {
        _graphPosition = graphPosition;

        this.transform.name = $"Node({_graphPosition})";

        this.transform.localPosition = new Vector3(
            _graphPosition.x * cellSize + cellSize * 0.5f,
            this.transform.position.y,
            _graphPosition.z * cellSize + cellSize * 0.5f);

        this.transform.localScale = new Vector3(1f * cellSize, 1f, 1f * cellSize);

        _debugText.text = _graphPosition.ToString();
    }

    public void SetNodeViewColor(Color color)
    {
        _tileRenderer.material.SetColor("_CellColor", color);
    }

    public void ShowGraphPosition()
    {
        _canvas.SetActive(true);
    }

    public void HideGraphPosition()
    {
        _canvas.SetActive(false);
    }
}
