using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class NodeView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_gText;
    [SerializeField] private TextMeshProUGUI m_fText;

    public GameObject _tile;
    public GameObject _arrow;
    

    [Range(0, 0.5f)]
    public float _borderSize = 0.15f;

    public Node _node;

    public void Init(Node node)
    {
        _node = node;
        if(_tile != null)
        {
            this.gameObject.name = $"Node({_node._graphPosition})";
            this.gameObject.transform.position = node._position;

            _tile.transform.localScale = new Vector3(1f - _borderSize, 1f, 1f - _borderSize);
        }

        EnableObject(_arrow, false);
        m_gText.text = node._distanceTravelled.ToString();
    }

    private void ColorNode(Color color, GameObject nodeObject)
    {
        if(nodeObject!= null)
        {
            Renderer nodeObjectRenderer = nodeObject.GetComponent<Renderer>();
            nodeObjectRenderer.material.color = color;
        }
    }

    public void ColorNode(Color color)
    {
        ColorNode(color, _tile);
    }

    public void ColorNodeDefaultColor()
    {
        ColorNode(MapData.GetColorFromTerrainCost(_node._terrainCost));
    }

    private void EnableObject(GameObject go, bool state)
    {
        go.SetActive(state);
    }

    public void SetText()
    {
        m_gText.text = _node._graphPosition.ToString();
        m_fText.text = _node._isBlocked.ToString();
    }

    public void ShowArrow(Color color)
    {
        if(_node != null && _arrow != null && _node._previous != null)
        {
            EnableObject(_arrow, true);

            Vector3 dirToPrevious = _node._previous._position - _node._position;
            dirToPrevious.Normalize();
            _arrow.transform.rotation = Quaternion.LookRotation(dirToPrevious);

            Renderer arrowRenderer = _arrow.GetComponent<Renderer>(); ;
            if(arrowRenderer != null)
            {
                arrowRenderer.material.color = color;
            }
        }
    }
}
