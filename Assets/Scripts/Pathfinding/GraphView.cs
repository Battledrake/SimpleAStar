using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    [SerializeField] private NodeView _nodeViewPrefab;

    private NodeView[,] _nodeViews;
    private Graph _graph;

    public void Init(Graph graph)
    {
        if (graph == null)
        {
            Debug.LogError("GRAPHVIEW No graph to Initialize");
        }

        _graph = graph;

        _nodeViews = new NodeView[graph.Width, graph.Height];

        GameObject nodeViewParent = new GameObject("[NodeViews]");

        foreach (Node node in _graph.Nodes)
        {
            NodeView nodeView = Instantiate<NodeView>(_nodeViewPrefab, nodeViewParent.transform);

            nodeView.Init(node._graphPosition, _graph.CellSize);
            _nodeViews[node._graphPosition.x, node._graphPosition.z] = nodeView;
            nodeView.SetViewColorFromIsBlocked(false);
        }
    }

    public void SetViewColorFromIsBlocked(GraphPosition graphPosition, bool isBlocked)
    {
        if(_graph.IsWithinBounds(graphPosition) && IsValidNodeView(graphPosition))
        {
            _nodeViews[graphPosition.x, graphPosition.z].SetViewColorFromIsBlocked(isBlocked);
        }
    }

    private bool IsValidNodeView(GraphPosition graphPosition)
    {
        return _nodeViews[graphPosition.x, graphPosition.z] != null;
    }
}
