using UnityEngine;

public class GraphView<T> where T : Node<T>
{
    private NodeView[,] _nodeViews;
    private Graph<T> _graph;
    private GameObject _nodeViewContainer;
    private bool _showGraphPositions;

    public GraphView(Graph<T> graph, int cellSize, NodeView nodeViewPrefab, Transform ownerTransform)
    {
        if (graph == null)
        {
            Debug.LogError("GRAPHVIEW No graph to Initialize");
        }

        _graph = graph;

        _nodeViews = new NodeView[graph.Width, graph.Height];

        _nodeViewContainer = new GameObject("[NodeViews]");
        _nodeViewContainer.transform.parent = ownerTransform;
        _nodeViewContainer.transform.localPosition = new Vector3(0f, 0.01f, 0f);

        foreach (T node in _graph.Nodes)
        {
            NodeView nodeView = GameObject.Instantiate<NodeView>(nodeViewPrefab, _nodeViewContainer.transform);

            nodeView.Init(node._graphPosition, cellSize);
            _nodeViews[node._graphPosition.x, node._graphPosition.z] = nodeView;
        }

        if (_showGraphPositions)
            ShowGraphPositions();
    }

    public void SetNodeViewColor(GraphPosition graphPosition, Color color)
    {
        if (IsValidNodeView(graphPosition))
        {
            _nodeViews[graphPosition.x, graphPosition.z].SetNodeViewColor(color);
        }
    }

    private bool IsValidNodeView(GraphPosition graphPosition)
    {
        return _nodeViews[graphPosition.x, graphPosition.z] != null;
    }

    public void HideGraphView()
    {
        _nodeViewContainer.SetActive(false);
    }

    public void ShowGraphView()
    {
        _nodeViewContainer.SetActive(true);
    }

    public void ShowGraphPositions()
    {
        foreach (NodeView node in _nodeViews)
        {
            node.ShowGraphPosition();
        }
    }

    public void HideGraphPositions()
    {
        foreach (NodeView node in _nodeViews)
        {
            node.HideGraphPosition();
        }
    }
}
