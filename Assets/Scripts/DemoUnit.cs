using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUnit : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _rotateSpeed = 10f;
    [Tooltip("Toggle for showing debug path")]
    [SerializeField] private bool _drawDebugPath;
    [Tooltip("How many spaces should this unit be allowed to move. Only used for debug drawlines atm")]
    [SerializeField] private int _moveAllowance;

    private List<Vector3> _pathPositions;
    private int _currentPositionIndex;

    private bool _isMoving;

    private void Update()
    {
        if (_isMoving)
        {
            if (_drawDebugPath)
                DrawDebugPath();

            Vector3 targetPosition = _pathPositions[_currentPositionIndex];

            float stopDistance = 0.1f;

            if (Vector3.Distance(targetPosition, transform.position) > stopDistance)
            {
                Vector3 moveDirection = (targetPosition - transform.position).normalized;

                transform.position += moveDirection * _moveSpeed * Time.deltaTime;

                Quaternion rotateDirection = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotateDirection, _rotateSpeed * Time.deltaTime);
            }
            else
            {
                _currentPositionIndex++;
                if (_currentPositionIndex >= _pathPositions.Count)
                {
                    _isMoving = false;
                }
            }
        }
    }

    private void DrawDebugPath()
    {
        for (int i = _pathPositions.Count - 1; i >= _currentPositionIndex; i--)
        {
            Vector3 lastPosition = this.transform.position;
            if(i > _currentPositionIndex)
            {
                lastPosition = _pathPositions[i - 1];
            }
            Color activeColor = i < _moveAllowance ? Color.green : Color.red;
            Debug.DrawLine(_pathPositions[i], lastPosition, activeColor);
        }
    }

    public void Move(List<Vector3> pathPositions)
    {
        if (pathPositions.Count <= 0) return;

        _pathPositions = pathPositions;

        _currentPositionIndex = 0;

        _isMoving = true;
    }
}
