using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUnit : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _rotateSpeed = 10f;

    private List<Vector3> _pathPositions;
    private int _currentPositionIndex;

    private bool _isMoving;

    private void Update()
    {
        if (_isMoving)
        {
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

    public void Move(List<Node> pathNodes)
    {
        _pathPositions = new List<Vector3>();

        foreach (Node node in pathNodes)
        {
            Debug.Log(node._position);
            _pathPositions.Add(node._position);
            _currentPositionIndex = 0;
        }
        _isMoving = true;
    }
}
