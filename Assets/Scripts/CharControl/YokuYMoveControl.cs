using System;
using Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class YokuYMoveControl : MonoBehaviour
{

    private FSM<CharMoveState> _fsm = new FSM<CharMoveState>();
    private Vector2 _moveVec2;
    private Transform _transform;
    public float rotateSpeed = 100f;

    public CharacterController characterController;
    public Animator animator;

    public float walkingSpeed = 1.6f;
    private float _targetSpeed;
    private static readonly int WalkingSpeed = Animator.StringToHash("WalkingSpeed");

    private void Awake()
    {
        _transform = transform;
        _fsm.State(CharMoveState.Idle).OnEnter(()=>{Debug.Log("Idle OnEnter");});
        _fsm.StartState(CharMoveState.Idle);

        _fsm.State(CharMoveState.Moving)
            .OnUpdate(() =>
            {
                RotatePlayer();
                MovePlayer();
            })
            .OnExit(() =>
            {
                _targetSpeed = 0f;
                animator.SetFloat(WalkingSpeed, 0.0f);
            });
    }

    private void Update()
    {
        _fsm.Update();
    }

    private void FixedUpdate()
    {
        _fsm.FixUpdate();
    }

    /// <summary>
    /// 角色移动事件
    /// </summary>
    /// <param name="ctx"></param>
    public void OnMoveInput(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Started) return;

        if (ctx.phase == InputActionPhase.Canceled)
        {
            _fsm.ChangeState(CharMoveState.Idle);
        }
        else
        {
            _moveVec2 = ctx.ReadValue<Vector2>();
            _fsm.ChangeState(CharMoveState.Moving);
        }
    }

    private void OnAnimatorMove()
    {
        characterController.SimpleMove(animator.deltaPosition);// 移动
    }

    private void MovePlayer()
    {
        animator.SetFloat(WalkingSpeed, _targetSpeed);
        _targetSpeed = Mathf.Lerp(_targetSpeed, walkingSpeed, 0.5f);// 插值
    }

    private void RotatePlayer()
    {
        var rotateVec = new Vector3(_moveVec2.x, 0, _moveVec2.y);
        // 获取方向
        Quaternion target = Quaternion.LookRotation(rotateVec, Vector3.up);
        // 转向
       _transform.rotation = Quaternion.RotateTowards(_transform.rotation, 
           target, rotateSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        _fsm.Clear();
    }
}

public enum CharMoveState
{
    Idle,
    Moving
}