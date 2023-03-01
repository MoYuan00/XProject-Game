using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlBasic : MonoBehaviour
{
    #region References Params

    [Header("References")] public Transform player; // 玩家节点，需要控制该节点位置。达到移动的目的。
    public Transform playerObj; // 玩家模型，需要控制模型的旋转，达到旋转的目的。
    public Transform orientation; // 当前移动正方向, 可以随时修改，比如以相机方向移动，或者以鼠标输入移动。
    public Rigidbody rg;

    #endregion

    /// 
    private Vector2 _inputDir;

    private Vector2 _lastMousePos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CameraFollow();
        RotateCamera();
    }

    #region Camera

    public Transform cameraPos;
    public Transform camera;
    private float yRotation;
    private float xRotation;
    public float senX = 200f;
    public float senY = 200f;

    private void RotateCamera()
    {
        // Debug.Log(_inputDir);
        _inputDir.x = Input.GetAxisRaw("Mouse X");
        _inputDir.y = Input.GetAxisRaw("Mouse Y");
        // Debug.Log($"input:{_inputDir}");
        if (_inputDir == Vector2.zero) return;
        yRotation += _inputDir.x * Time.deltaTime * senY;

        xRotation -= _inputDir.y * Time.deltaTime * senX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        camera.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void CameraFollow()
    {
        camera.position = cameraPos.position;
    }

    #endregion


    #region Input

    public void OnMovingInput(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed) return;
        var pos = ctx.ReadValue<Vector2>();
        if (_lastMousePos == Vector2.zero) _inputDir = Vector2.zero;
        else _inputDir = (pos - _lastMousePos) * 0.01f;
        _lastMousePos = pos;
    }

    #endregion
}