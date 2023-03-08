using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[Obsolete]
public class Camera3rdSyncPlayerRotate : MonoBehaviour
{
    public Transform orientation; // 方向 forward, 空物体仅用来 记录相机和视角的方向
    public Transform combatLookAt; // 方向 forward, 空物体仅用来 记录相机在WalkingAim模式下目标方向
    public Transform player;
    public Transform playerObj;
    public float rotationSpeed = 7f;

    private Vector2 _moveInput;

    [Header("References Camera")]
    public GameObject cameraFree;
    public GameObject cameraFreeHigh;
    public GameObject cameraWalkingAim;

    private Transform _currentCameraTrans;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CameraManager.OnCameraModeChange.AddListener(OnChangeCameraMode);
        OnChangeCameraMode(CameraManager.CurrentCameraMode);
    }

    private void OnChangeCameraMode(CameraMode cameraState)
    {
        switch (cameraState)
        {
            case CameraMode.Free:
                _currentCameraTrans = cameraFree.transform;
                break;
            case CameraMode.FreeHigh:
                _currentCameraTrans = cameraFreeHigh.transform;
                break;
            case CameraMode.WalkingAim:
                _currentCameraTrans = cameraWalkingAim.transform;
                break;
        }
    }


    private void Update()
    {
        var cameraMode = CameraManager.CurrentCameraMode;
        var cameraPos = _currentCameraTrans.position;
        
        
        if (cameraMode is CameraMode.Free or CameraMode.FreeHigh)
        {
            // 旋转角色
            Vector3 inputDir = orientation.forward * _moveInput.y
                               + orientation.right * _moveInput.x;
            if (inputDir == Vector3.zero) return;

            var playerPos = player.position;
            Vector3 viewDir = new Vector3(playerPos.x, 0, playerPos.z) - new Vector3(cameraPos.x, 0, cameraPos.z);
            orientation.forward = viewDir.normalized;

            Debug.DrawLine(playerPos, playerPos + inputDir);
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        else if (cameraMode is CameraMode.WalkingAim)
        {
            var combatPos = combatLookAt.position;
            Vector3 combatLookAtDir = new Vector3(combatPos.x, 0, combatPos.z) - new Vector3(cameraPos.x, 0, cameraPos.z);
            var dir = combatLookAtDir.normalized;
            // Debug.DrawLine(playerObj.position, playerObj.position + orientation.forward);
            orientation.forward = dir;
            playerObj.forward = dir;
        }
    }

    public void OnMoveInput(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }
}