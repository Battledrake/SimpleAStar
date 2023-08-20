using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    public MapData _mapData;
    public Graph _graph;
    private GraphView _graphView;

    public Pathfinder _pathfinder;
    public int _startX = 1;
    public int _startY = 15;
    public int _goalX = 13;
    public int _goalY = 0;

    private List<Node> _pathNodes;

    public float _timeStep = 0.1f;

    private void Start()
    {
        if (_mapData != null && _graph != null)
        {
            int[,] mapInstance = _mapData.MakeMap();
            _graph.Init(mapInstance);

            _graphView = _graph.gameObject.GetComponent<GraphView>();

            if (_graphView != null)
            {
                _graphView.Init(_graph);
            }

            if (_graph.IsWithinBounds(new GraphPosition(_startX, _startY))
                && _graph.IsWithinBounds(new GraphPosition(_goalX, _goalY))
                && _pathfinder != null)
            {
                _graphView.ResetColors();

                Node startNode = _graph.Nodes[_startX, _startY];
                Node goalNode = _graph.Nodes[_goalX, _goalY];

                List<Node> myPath = new List<Node>();

                PathResult checkResult = _pathfinder.FindPath(startNode, goalNode, _graph, ref myPath);

                _graphView.ColorNodes(myPath, Color.cyan);
                Debug.Log(checkResult);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitResult))
            {
                NodeView nodeView = hitResult.collider.gameObject.GetComponentInParent<NodeView>();
                if (nodeView != null)
                {
                    _graphView.ResetColors();
                    _pathNodes = new List<Node>();

                    Node startNode = _graph.Nodes[_startX, _startY];

                    List<Node> myPath = new List<Node>();

                    PathResult checkResult = _pathfinder.FindPath(startNode, nodeView._node, _graph, ref myPath);
                    for(int i = 0; i < myPath.Count; ++i)
                    {
                        float distanceSoFar = myPath[i]._distanceTravelled;
                        if (distanceSoFar < 7)
                            _graphView.ColorNode(myPath[i], Color.cyan);
                        else
                            _graphView.ColorNode(myPath[i], Color.red);
                    }
                    _pathNodes = myPath;
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitResult))
            {
                NodeView nodeView = hitResult.collider.gameObject.GetComponentInParent<NodeView>();
                if (nodeView != null)
                {
                    Node startNode = _pathNodes.Last();

                    List<Node> myPath = new List<Node>();

                    PathResult checkResult = _pathfinder.FindPath(startNode, nodeView._node, _graph, ref myPath);

                    _pathNodes.AddRange(myPath);
                    _graphView.ColorNodes(_pathNodes, Color.red);
                    Debug.Log(_pathNodes.Count);
                }
            }
        }
    }
}
