using System;
using System.Collections;
using System.Collections.Generic;
using CharControl;
using UnityEngine;
using UnityEngine.InputSystem;

public class TownDownController : MonoBehaviour
{
    public Animator animator;

    private Vector2 _inputMoveVec2;
    private PlayerStateManager _playerStateManager;
    public CharMoveState state;
    private bool _isRunning;

    public Transform orientation;
    public float rotateSpeed = 300f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _playerStateManager = new PlayerStateManager();
        orientation = transform;
    }

    private void Update()
    {
        RotatePlayer();
    }

    private void GetPlayerMoveInput(InputAction.CallbackContext ctx)
    {
        _inputMoveVec2 = ctx.ReadValue<Vector2>();
    }

    private void GetPlayerRunningInput(InputAction.CallbackContext ctx)
    {
        _isRunning = ctx.ReadValue<float>() > 0;
    }

    private void RotatePlayer()
    {
        if(_inputMoveVec2 == Vector2.zero) return;
        var playerMovement = new Vector3(_inputMoveVec2.x, 0, _inputMoveVec2.y);

        Quaternion target = Quaternion.LookRotation(playerMovement, Vector3.up);
        orientation.rotation = Quaternion.RotateTowards(orientation.rotation, target,
            rotateSpeed * Time.deltaTime);
    }
}