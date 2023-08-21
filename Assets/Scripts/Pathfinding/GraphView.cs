using UnityEngine;

public class GraphView : MonoBehaviour
{
    [SerializeField] private NodeView _nodeViewPrefab;

    private NodeView[,] _nodeViews;
    private Graph _graph;
    private GameObject _nodeViewContainer;

    public void Init(Graph graph, int cellSize)
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

        foreach (Node node in _graph.Nodes)
        {
            NodeView nodeView = Instantiate<NodeView>(_nodeViewPrefab, _nodeViewContainer.transform);

            nodeView.Init(node._graphPosition, cellSize);
            _nodeViews[node._graphPosition.x, node._graphPosition.z] = nodeView;
            nodeView.SetNodeViewColor(Color.grey);
        }
    }

    public void SetNodeViewColor(GraphPosition graphPosition, Color color)
    {
        if(_graph.IsWithinBounds(graphPosition) && IsValidNodeView(graphPosition))
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
}
