using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Graph))]
public class GraphView : MonoBehaviour
{
    [SerializeField] private NodeView _nodeViewPrefab;
    [SerializeField] private GameObject _worldGrid;

    [SerializeField] private Color _openColor;
    [SerializeField] private Color _blockedColor;
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _inRangePathColor;
    [SerializeField] private Color _outRangePathColor;
    public NodeView[,] _nodeViews;

    private Graph _graph;

    public void Init(Graph graph)
    {
        if (graph == null)
        {
            Debug.LogError("GRAPHVIEW No graph to Initialize");
        }

        _graph = graph;

        //_nodeViews = new NodeView[graph.Width, graph.Height];

        //GameObject nodeViewParent = new GameObject("[NodeViews]");

        //foreach (Node node in graph.Nodes)
        //{
        //    NodeView nodeView = Instantiate<NodeView>(_nodeViewPrefab, nodeViewParent.transform);

        //    if (nodeView != null)
        //    {
        //        nodeView.Init(node);
        //        _nodeViews[node._graphPosition.x, node._graphPosition.z] = nodeView;
        //        if (node._isBlocked)
        //        {
        //            nodeView.ColorNode(Color.black);
        //        }
        //        else
        //        {
        //            Color originalColor = MapData.GetColorFromTerrainCost(node._terrainCost);
        //            nodeView.ColorNode(originalColor);
        //        }
        //    }
        //}
    }

    public void SetInRangePathNode(Node pathNode)
    {
        if (_worldGrid == null) return;

        Renderer gridRenderer = _worldGrid.GetComponent<Renderer>();
        gridRenderer.material.SetInt("_SelectedCellX", pathNode._graphPosition.x);
        gridRenderer.material.SetInt("_SelectedCellY", pathNode._graphPosition.z);
        gridRenderer.material.SetInt("_SelectedCell", 1);
    }

    public void ResetColors()
    {
        foreach (NodeView nodeView in _nodeViews)
        {
            nodeView.ColorNodeDefaultColor();
        }
    }

    public void ColorNode(Node node, Color color)
    {
        NodeView nodeView = _nodeViews[node._graphPosition.x, node._graphPosition.z];
        if (nodeView != null)
        {
            nodeView.ColorNode(color);
        }
    }

    public void ColorNodes(List<Node> nodes, Color color, bool lerpColor = false, float lerpValue = 0.5f)
    {
        foreach (Node node in nodes)
        {
            if (node != null)
            {
                NodeView nodeView = _nodeViews[node._graphPosition.x, node._graphPosition.z];
                Color newColor = color;
                if (lerpColor)
                {
                    Color originalColor = MapData.GetColorFromTerrainCost(node._terrainCost);
                    newColor = Color.Lerp(originalColor, newColor, lerpValue);
                }

                if (nodeView != null)
                {
                    nodeView.ColorNode(newColor);
                }
            }
        }
    }

    public void ShowNodeArrow(Node node, Color color)
    {
        if (node != null)
        {
            NodeView nodeView = _nodeViews[node._graphPosition.x, node._graphPosition.z];
            if (nodeView != null)
            {
                nodeView.ShowArrow(color);
                nodeView.SetText();
            }
        }
    }

    public void ShowNodeArrows(List<Node> nodes, Color color)
    {
        foreach (Node node in nodes)
        {
            ShowNodeArrow(node, color);
        }
    }
}
