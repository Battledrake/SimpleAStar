using UnityEngine;

public class GraphView : MonoBehaviour
{
    [SerializeField, RuntimeReadOnly] private NodeView _nodeViewPrefab;
    [Tooltip("Show graph positions on each Node. GraphView must be enabled.")]
    [SerializeField, RuntimeReadOnly] private bool _showGraphPositions;

    private NodeView[,] _nodeViews;
    private Graph<PathNode> _graph;
    private GameObject _nodeViewContainer;

    public void Init(Graph<PathNode> graph, int cellSize)
    {
        if (graph == null)
        {
            Debug.LogError("GRAPHVIEW No graph to Initialize");
        }

        _graph = graph;

        _nodeViews = new NodeView[graph.Width, graph.Height];

        _nodeViewContainer = new GameObject("[NodeViews]");
        _nodeViewContainer.transform.parent = this.transform;
        _nodeViewContainer.transform.localPosition = new Vector3(0f, 0.01f, 0f);

        foreach (PathNode node in _graph.Nodes)
        {
            NodeView nodeView = Instantiate<NodeView>(_nodeViewPrefab, _nodeViewContainer.transform);

            nodeView.Init(node._graphPosition, cellSize);
            _nodeViews[node._graphPosition.x, node._graphPosition.z] = nodeView;
            nodeView.SetNodeViewColor(Color.grey);
        }

        if (_showGraphPositions)
            ShowGraphPositions();
    }

    public void SetNodeViewColor(GraphPosition graphPosition, Color color)
    {
        if(IsValidNodeView(graphPosition))
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
        foreach(NodeView node in _nodeViews)
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
