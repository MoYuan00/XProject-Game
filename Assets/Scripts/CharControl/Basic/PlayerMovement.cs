using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Transform orientation;
    public float playerHeight;
    [Header("Movement")] public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    [Tooltip("在地面时的阻尼, 用于控制角色无限加速")] public float groundDrag;

    [Header("Jump")] public float jumpForce;
    [Tooltip("跳跃冷却")] public float jumpCooldown;
    [Tooltip("跳跃在空中时的阻尼倍数-希望空中有阻尼")] public float airMultiplier;

    private bool _readyToJump = true;

    [Header("Slope 斜坡")] public float maxSlopeAngle = 45f;
    private RaycastHit _slopeHitInfo;

    [Header("Ground Check")] public LayerMask whatIsGround;
    private bool _grounded;


    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _moveDirection;

    public MovementState state;

    private bool _existsSlope = false; // 解决斜坡控制速度，导致跳跃出问题

   public enum MovementState
    {
        Air,
        Sprinting,
        Walking
    }

    void Start()
    {
        rb.freezeRotation = true;
    }

    private void Update()
    {
        GetInput();
        StateHandler();
        GroundCheck();
        SpeedControl();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(KeyCode.Space) && _readyToJump && _grounded)
        {
            _readyToJump = false;
            Jump();
            Invoke(nameof(ResetJumpCooldown), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        _moveDirection = orientation.forward * _verticalInput
                         + orientation.right * _horizontalInput;

        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection() * (moveSpeed * 10f), ForceMode.Force);
            if(rb.velocity.y > 0) // 防止斜坡运动时弹起
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (_grounded)
            rb.AddForce(_moveDirection * (moveSpeed * 8f), ForceMode.Force);
        else
            rb.AddForce(_moveDirection * (moveSpeed * 8f * airMultiplier), ForceMode.Force);
        
        // 斜坡上运动时关闭重力，防止下滑
        rb.useGravity = !OnSlope();
    }

    private void GroundCheck()
    {
        var rayLength = playerHeight * 0.5f + 0.2f;
        _grounded = Physics.Raycast(transform.position, Vector3.down, rayLength, whatIsGround);

        Debug.DrawLine(transform.position, transform.position + Vector3.down * rayLength, Color.green);

        if (_grounded) rb.drag = groundDrag;
        else rb.drag = 0;
    }

    private void SpeedControl()
    {
        if (OnSlope() && !_existsSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            var velocity = rb.velocity;
            var flatSpeed = new Vector3(velocity.x, 0f, velocity.z);

            if (flatSpeed.magnitude > moveSpeed)
            {
                Vector3 limitedSpeed = flatSpeed.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedSpeed.x, velocity.y, limitedSpeed.z);
            }
        }
    }

    private void Jump()
    {
        _existsSlope = true;
        
        // reset y velocity
        var velocity = rb.velocity;
        velocity = new Vector3(velocity.x, 0f, velocity.y);
        rb.velocity = velocity;

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJumpCooldown()
    {
        _readyToJump = true;

        _existsSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHitInfo, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHitInfo.normal); // 斜坡的角度
            Debug.Log($"{angle}");
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        // 对水平的移动方向进行 投影，投影到 以normal为法线的斜面上。只需要方向
        var vector3 = Vector3.ProjectOnPlane(_moveDirection, _slopeHitInfo.normal).normalized;
        Debug.DrawLine(transform.position, transform.position + vector3 * 2f, Color.red);
        return vector3;
        
    }
    private void StateHandler()
    {
        if (_grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.Sprinting;
            moveSpeed = sprintSpeed;
        }
        else if(_grounded)
        {
            state = MovementState.Walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.Air;
        }
    }


}