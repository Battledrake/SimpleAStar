using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PathResult
{
    SearchFail,
    SearchSuccess,
    GoalUnreachable
}

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance;

    private enum CalculationType
    {
        Chebyshev,
        Diagonal,
        DiagonalShortcut,
        Euclidean,
        Manhattan
    }

    private enum TraversalType
    {
        AllNonBlocked,
        NoSharpDiagonals,
        SharpDiagonals,
    }

    [Tooltip("Formulas for calculating estimated cost of travel from current node to goal.")]
    [SerializeField] private CalculationType _heuristicCost;
    [Tooltip("Formulas for calculating cost of travel between each node.")]
    [SerializeField] private CalculationType _traversalCost;
    [Tooltip("How do we traverse open nodes near and between walls when moving diagonally. Only applies to 8 direction movement on grid")]
    [SerializeField] private TraversalType _traversalType;
    [Tooltip("How much does Heuristics effect solution. 1 is balanced G and H. Lower guarantees shortest path at cost of slow processing(Dijkstra). Higher becomes faster with longer path(Greedy Best First)")]
    [SerializeField] private float _heuristicScale = 1f;
    [Tooltip("Allow a returned path that does not reach the end goal")]
    [SerializeField] private bool _allowPartialSolution = false;
    [Tooltip("Should we revisit closed nodes for a possibly short path at the cost of performance?")]
    [SerializeField] private bool _ignoreClosed = true;
    [Tooltip("Should we include the start node in the end result path?")]
    [SerializeField] private bool _includeStartNodeInPath;

    private PriorityQueue<Node> _frontierNodes;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError($"Duplicate Pathfinders found: {this.transform} - {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public PathResult FindPath(GraphPosition startPosition, GraphPosition endPosition, Graph graph, out List<GraphPosition> outPath)
    {
        Node startNode = graph.GetNodeFromGraphPosition(startPosition);
        Node goalNode = graph.GetNodeFromGraphPosition(endPosition);

        outPath = new List<GraphPosition>();

        if (startNode == null || goalNode == null)
            return PathResult.SearchFail;
        if (startNode == goalNode)
            return PathResult.SearchSuccess;

        graph.ResetNodes();
        _frontierNodes = new PriorityQueue<Node>();

        float timeStart = Time.realtimeSinceStartup;

        startNode._traversalCost = 0;
        startNode._totalCost = GetHeuristicCost(startNode, goalNode) * _heuristicScale;

        _frontierNodes.Enqueue(startNode);
        startNode._isOpened = true;

        Node bestNode = startNode;
        float bestNodeCost = startNode._totalCost;
        PathResult pathResult = PathResult.SearchSuccess;

        bool processNodes = true;
        while (_frontierNodes.Count > 0 && processNodes)
        {
            processNodes = ProcessSingleNode(goalNode, graph, ref bestNode, ref bestNodeCost);
        }

        if (bestNodeCost != 0f)
            pathResult = PathResult.GoalUnreachable;

        if (pathResult == PathResult.SearchSuccess || _allowPartialSolution)
        {
            outPath = ConvertPathToGraphPositions(bestNode);

            Debug.Log($"PATHFINDER path length = {outPath.Count()}");
        }

        Debug.Log("PATHFINDER SearchRoutine: elapsed time = " + (Time.realtimeSinceStartup - timeStart).ToString() + " seconds");
        return pathResult;
    }

    private bool ProcessSingleNode(Node goalNode, Graph graph, ref Node bestNode, ref float bestNodeCost)
    {
        Node currentNode = _frontierNodes.Dequeue();
        currentNode._isClosed = true;

        if (currentNode == goalNode)
        {
            bestNode = currentNode;
            bestNodeCost = 0f;
            return false;
        }

        for (int i = 0; i < currentNode._neighbors.Count; i++)
        {
            Node neighborNode = currentNode._neighbors[i];

            if (neighborNode == currentNode._previous || !IsTraversalAllowed(currentNode, neighborNode, graph))
                continue;

            if (_ignoreClosed && neighborNode._isClosed)
                continue;

            float newTraversalCost = GetTraversalCost(currentNode, neighborNode) + currentNode._traversalCost;
            float newHeuristic = GetHeuristicCost(neighborNode, goalNode) * _heuristicScale;
            float newTotalCost = newTraversalCost + newHeuristic;

            if (newTotalCost >= neighborNode._totalCost)
                continue;

            neighborNode._traversalCost = newTraversalCost;
            neighborNode._totalCost = newTotalCost;
            neighborNode._previous = currentNode;
            neighborNode._isClosed = false;

            if (!neighborNode._isOpened)
            {
                _frontierNodes.Enqueue(neighborNode);
                neighborNode._isOpened = true;
            }

            if (newHeuristic < bestNodeCost)
            {
                bestNodeCost = newHeuristic;
                bestNode = neighborNode;
            }
        }
        return true;
    }

    public List<GraphPosition> ConvertPathToGraphPositions(Node endNode)
    {
        List<GraphPosition> graphPositions = new List<GraphPosition>();

        graphPositions.Add(endNode._graphPosition);

        Node currentNode = endNode;

        int pathLength = graphPositions.Count;
        while(currentNode._previous != null)
        {
            graphPositions.Add(currentNode._previous._graphPosition);
            currentNode = currentNode._previous;
            pathLength++;
        }

        if (!_includeStartNodeInPath)
            graphPositions.RemoveAt(pathLength - 1);

        graphPositions.Reverse();

        return graphPositions;
    }

    private float GetHeuristicCost(Node source, Node target)
    {
        return GetCalculationFromType(_heuristicCost, source, target);
    }

    private float GetTraversalCost(Node source, Node target)
    {
        return GetCalculationFromType(_traversalCost, source, target) + target._terrainCost;
    }

    private float GetCalculationFromType(CalculationType calculationType, Node source, Node target)
    {
        switch (calculationType)
        {
            case CalculationType.Chebyshev:
                return StaticFormulas.Chebyshev(source, target);
            case CalculationType.Diagonal:
                return StaticFormulas.Diagonal(source, target);
            case CalculationType.DiagonalShortcut:
                return StaticFormulas.DiagonalShortcut(source, target);
            case CalculationType.Euclidean:
                return StaticFormulas.Euclidean(source, target);
            case CalculationType.Manhattan:
                return StaticFormulas.Euclidean(source, target);
        }
        return 0f;
    }

    private bool IsTraversalAllowed(Node source, Node target, Graph graph)
    {
        if (target._isBlocked)
        {
            Debug.LogWarning("PATHFINDER: isBlocked was true, this shouldn't happen.");
            return false;
        }

        if (_traversalType == TraversalType.AllNonBlocked) return true;

        int srcX = source._graphPosition.x;
        int srcZ = source._graphPosition.z;
        GraphPosition distance = target._graphPosition - source._graphPosition;

        if (Mathf.Abs(distance.x * distance.z) == 1)
        {
            if (_traversalType == TraversalType.NoSharpDiagonals)
            {
                if (!graph.IsGraphPositionTraversable(new GraphPosition(srcX + distance.x, srcZ))
                 || !graph.IsGraphPositionTraversable(new GraphPosition(srcX, srcZ + distance.z)))
                {
                    return false;
                }
            }
            else
            {
                if (!graph.IsGraphPositionTraversable(new GraphPosition(srcX + distance.x, srcZ))
                 && !graph.IsGraphPositionTraversable(new GraphPosition(srcX, srcZ + distance.z)))
                {
                    return false;
                }
            }
        }
        return true;
    }
}
