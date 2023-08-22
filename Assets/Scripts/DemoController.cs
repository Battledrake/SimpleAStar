using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    [SerializeField] private LevelGrid _levelGrid;
    [SerializeField] private DemoUnit _demoUnit;

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
                    Vector3 startPosition = _demoUnit.transform.position;
                    Vector3 endPosition = hitResult.point;
                    PathResult checkResult = Pathfinder.Instance.FindPath(startPosition, endPosition, _levelGrid, out List<Vector3> pathPositions);
                    if (checkResult == PathResult.SearchSuccess || checkResult == PathResult.GoalUnreachable)
                    {

                        _demoUnit.Move(pathPositions);
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
            _levelGrid.SetBlockedNodeFromWorldPosition(worldPosition);
        else
            _levelGrid.SetUnblockedNodeFromWorldPosition(worldPosition);
    }
}
