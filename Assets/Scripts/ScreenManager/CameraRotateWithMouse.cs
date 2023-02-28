using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraRotateWithMouse : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    
    private Vector2 _lastMousePos;
    private Vector2 _mouseMovingDir;

    private void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void OnRotateCamera(InputAction.CallbackContext ctx)
    {
        var mousePos = ctx.ReadValue<Vector2>();
        _mouseMovingDir = mousePos - _lastMousePos;
        _lastMousePos = mousePos;
        Debug.Log($"OnRotateCamera:{_mouseMovingDir}");
    }

    private void RotateCamera()
    {
    }
}
