using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DemoCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _camera;

    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _rotateSpeed = 100f;
    [SerializeField] private float _zoomAmount = 5f;
    [SerializeField] private float _zoomSpeed = 50f;
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 30f;

    private CinemachineTransposer _cameraTransposer;
    private Vector3 _targetFollowOffset;

    private void Start()
    {
        this.transform.position = GameObject.Find("DemoUnit").transform.position;
        _cameraTransposer = _camera.GetCinemachineComponent<CinemachineTransposer>();
        _targetFollowOffset = _cameraTransposer.m_FollowOffset;
    }

    private void Update()
    {
        Vector3 movementVector = Vector3.zero;
        Vector3 rotationVector = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movementVector.z += 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementVector.z -= 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementVector.x -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementVector.x += 1f;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y += 1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y -= 1f;
        }

        this.transform.position += this.transform.TransformDirection(movementVector * _moveSpeed * Time.deltaTime);
        this.transform.Rotate(rotationVector * _rotateSpeed * Time.deltaTime);

        _targetFollowOffset.y -= Input.mouseScrollDelta.y * _zoomAmount;
        _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, _minZoom, _maxZoom);
        Vector3 followOffset = Vector3.Lerp(_cameraTransposer.m_FollowOffset, _targetFollowOffset, _zoomSpeed * Time.deltaTime);

        _cameraTransposer.m_FollowOffset = followOffset;

    }
}
