using System;
using System.Collections;
using System.Collections.Generic;
using PlayerFramework;
using UnityEngine;

/// <summary>
/// 控制角色实际移动，修改角色位置或者速度达到移动的效果
/// </summary>
public class PlayerMovementManager : MonoBehaviour
{
    private CharacterController characterController;
    private Animator _animator;
    private Camera mainCamera;
    private PlayerAnimatorManager _animatorManager;

    [Header("速度配置")] public float rotateSpeed = 300f;
    public float gravity = -15f;
    public float walkSpeed = 3f;
    public float runSpeed = 5.5f;
    public float jumpMaxHeight = 1.0f;
    public float aimWalkSpeed = 3f;
    public float climbSpeed = 1f;

    public float speedY;
    public float airMoveSpeed = 2.0f;


    private Vector3 _animatorMovement;
    public float Speed => _animatorMovement.magnitude;

    private Vector3 _midairMovement;

    public bool isEnableRootMotionMove = true;
    public bool isSyncAnimatorSpeedY; // 是否使用动画里面的Y方向速度

    [Header("Aim")] public float rotateCameraSpeed = 100f;

    private void Awake()
    {
        mainCamera = Camera.main;
        _animatorManager = GetComponent<PlayerAnimatorManager>();
        characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void OnAnimatorMove()
    {
        if(!isEnableRootMotionMove) return;
        // 使用动画里面的速度
        Vector3 deltaMovement = _animator.deltaPosition;
        if (!isSyncAnimatorSpeedY)
        {
            deltaMovement.y = speedY * Time.deltaTime;
        }
        characterController.Move(deltaMovement);
    }


    public void AirMovementWithInput(Vector2 inputMoveVec2)
    {
        if (inputMoveVec2 == Vector2.zero)
        {
            _midairMovement = Vector3.zero;
            return;
        }

        _midairMovement = CalculateInputDirection(inputMoveVec2) * (airMoveSpeed * Time.deltaTime);
        characterController.Move(_midairMovement);
    }

    public void ClimbWithInput(Vector2 inputMoveVec2)
    {
        if (inputMoveVec2 == Vector2.zero)
        {
            speedY = 0f;
            return;
        }
        var y = inputMoveVec2.y ;
        var move =  new Vector3(0, climbSpeed , 0);
        if (y < 0) move = -move;
        // 移动
        speedY = move.y;
        characterController.Move(move * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (_midairMovement != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _midairMovement * 10f);
        }
    }


    public float RotateWithInputCameraSpace(Vector2 inputMoveVec2)
    {
        _animatorMovement = transform.InverseTransformVector(CalculateInputDirection(inputMoveVec2));
        float rad = inputMoveVec2 == Vector2.zero
            ? 0
            : Mathf.Atan2(_animatorMovement.x, _animatorMovement.z); // 转向方向 - 弧度
        // 使用OnAnimatorMove后转向也会失效，可以使用_characterController控制
        _animatorManager.Rotate(rad);
        transform.Rotate(0, rad * rotateSpeed * Time.deltaTime, 0f);
        return rad;
    }

    public void RotateFollowMouse(Vector2 mouseDir)
    {
        float yRotation = transform.rotation.eulerAngles.y;
        yRotation += mouseDir.x * Time.deltaTime * rotateCameraSpeed;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    // 计算出移动的方向
    private Vector3 CalculateInputDirection(Vector2 inputMoveVec2)
    {
        // 计算角色相对于相机方向的移动
        
        var mainTransform = mainCamera.transform;
        var forward = mainTransform.forward;
        
        Vector3 camForwardProjection = new Vector3(forward.x, 0, forward.z).normalized;
        
        return camForwardProjection * inputMoveVec2.y
               + mainTransform.right * inputMoveVec2.x;
    }

    public void AmountGravityToSpeedY(float factor)
    {
        speedY += gravity * Time.deltaTime * factor;
    }

    public void Jump()
    {
        // 给与向上的速度
        speedY = Mathf.Sqrt(-2 * gravity * jumpMaxHeight);
    }
}