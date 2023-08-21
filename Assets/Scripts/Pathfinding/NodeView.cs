using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class NodeView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_gText;
    [SerializeField] private TextMeshProUGUI m_fText;

    [SerializeField] private GameObject _tile;
    [SerializeField] private GameObject _arrow;
    [SerializeField] private Renderer _tileRenderer;

    [SerializeField] private Color _openColor;
    [SerializeField] private Color _blockedColor;

    private GraphPosition _graphPosition;

    public void Init(GraphPosition graphPosition, int cellSize)
    {
        _graphPosition = graphPosition;

        if (_tile != null)
        {
            this.gameObject.name = $"Node({_graphPosition})";
            this.gameObject.transform.position = new Vector3(_graphPosition.x * cellSize, 0, _graphPosition.z * cellSize);

            this.transform.localScale = new Vector3(1f * cellSize, 1f, 1f * cellSize);
        }
        //m_gText.text = node._distanceTravelled.ToString();
    }

    public void SetViewColorFromIsBlocked(bool isBlocked)
    {
        Color setColor = isBlocked ? _blockedColor : _openColor;
        _tileRenderer.material.SetColor("_CellColor", setColor);
    }

    //public void SetText()
    //{
    //    m_gText.text = _node._graphPosition.ToString();
    //    m_fText.text = _node._isBlocked.ToString();
    //}
}
