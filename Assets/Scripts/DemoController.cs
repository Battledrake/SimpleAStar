using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    [SerializeField] private AStarGraphController _graphController;
    [SerializeField] private DemoUnit _demoUnit;

    //TODO: This will be used to stack paths for a waypoint like pathing
    private List<GraphPosition> _pathPositions;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (_demoUnit == null) return;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitResult))
                {
                    GraphPosition startPosition = _graphController.GetGraphPositionFromWorld(_demoUnit.transform.position);
                    GraphPosition endPosition = _graphController.GetGraphPositionFromWorld(hitResult.point);
                    PathResult checkResult = Pathfinder.Instance.FindPath(startPosition, endPosition, _graphController.GetGraph(), out List<GraphPosition> pathPositions);
                    if (checkResult == PathResult.SearchSuccess || checkResult == PathResult.GoalUnreachable)
                    {
                        _demoUnit.Move(_graphController.GetWorldPositionsFromGraphPositions(pathPositions));
                    }
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitResult))
                {
                    SetBlockedStateFromWorldPosition(hitResult.point, false);
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitResult))
            {
                SetBlockedStateFromWorldPosition(hitResult.point, true);
            }
        }
    }

    private void SetBlockedStateFromWorldPosition(Vector3 worldPosition, bool isBlocked)
    {
        if (isBlocked)
            _graphController.SetBlockedNodeFromWorldPosition(worldPosition);
        else
            _graphController.SetUnblockedNodeFromWorldPosition(worldPosition);
    }
}
