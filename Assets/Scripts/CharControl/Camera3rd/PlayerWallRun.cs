using System;
using CharControl;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallRun : MonoBehaviour
{
    public Transform orientation;
    public Rigidbody rigidbody;
    [Header("wallRunning")] public LayerMask wallRunningLayerMask;
    public float wallRunningFaceAngle = 30f;
    public float wallRunningSpeed;
    public float wallRunningDuration = 2f;
    public float wallRunningCooldown = 0.4f;
    public float wallRunningDistance = 0.7f;

    private bool _readyWallRunning = true;
    private float _wallRunningTime = 0f;
    private Vector2 _inputMove;


    private bool _wallRight;
    private bool _wallLeft;
    private RaycastHit _leftHit;
    private RaycastHit _rightHit;

    private PlayerStateManager _playerStateManager;
    private GroundChecker _groundChecker;

    public void Start()
    {
        _playerStateManager = GetComponent<PlayerStateManager>();
        _groundChecker = GetComponent<GroundChecker>();
    }

    public void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (_playerStateManager.Exists(CharMoveState.WallRunning))
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        _wallRight = Physics.Raycast(orientation.position, orientation.right, 
            out _rightHit, wallRunningDistance,
            wallRunningLayerMask);
        _wallLeft = Physics.Raycast(orientation.position, -orientation.right, 
            out _leftHit, wallRunningDistance,
            wallRunningLayerMask);
    }

    private void StateMachine()
    {
        // 如果可以墙跑
        if ((_wallRight || _wallLeft) && _inputMove.y > 0 && !_groundChecker.IsOnGround())
        {
            _playerStateManager.Append(CharMoveState.WallRunning);
        }
        else
        {
            rigidbody.useGravity = true;
            _playerStateManager.Remove(CharMoveState.WallRunning);
        }
    }


    private void WallRunningMovement()
    {
        var wallNormal = _wallRight ? _rightHit.normal : _leftHit.normal;
        var forward = Vector3.Cross(wallNormal, Vector3.up);

        if ((orientation.forward - wallNormal).magnitude > (orientation.forward - -wallNormal).magnitude)
        {
            forward = -forward;
        }
        Debug.Log($"one:{(orientation.forward - wallNormal).magnitude} to:{(orientation.forward - -wallNormal).magnitude}");
        Debug.DrawRay(orientation.position, forward);
        Debug.DrawRay(orientation.position, orientation.forward, Color.yellow);
        
        // 向前方施加速度
        rigidbody.AddForce(-wallNormal * 40f, ForceMode.Force);
        rigidbody.useGravity = false;

        var velocity = forward * wallRunningSpeed;
        rigidbody.velocity = Vector3.Lerp(rigidbody.velocity, velocity, 0.5f);
        // rigidbody.AddForce(forward * wallRunningSpeed, ForceMode.Force);
    }


    private void ResetWallRunning()
    {
        _readyWallRunning = true;
    }

    public void OnMoveInput(InputAction.CallbackContext ctx)
    {
        _inputMove = ctx.ReadValue<Vector2>();
    }
    
   
}